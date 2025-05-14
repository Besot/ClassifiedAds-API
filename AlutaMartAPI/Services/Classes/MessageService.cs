using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services
{
    public class MessageService(
        IUnitOfWork unitOfWork,
        IResponseService responseService) : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IResponseService _responseService = responseService;

        public async Task<ServiceResponse<GetConversationDTO>> GetConversationAsync(Guid conversationId, UserDTO currentUser)
        {
            // 1) Load the conversation and all related data
            var conversation = await _unitOfWork.Context.Conversations
                .AsNoTracking()
                .Include(c => c.Buyer).ThenInclude(b => b.Profile)
                .Include(c => c.Vendor)
                .Include(c => c.Ads)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return _responseService.ErrorResponse<GetConversationDTO>("Conversation not found");

            // 2) Ensure the user is a participant
            if (currentUser.BuyerId.HasValue && conversation.BuyerId != currentUser.BuyerId.Value &&
                currentUser.VendorId.HasValue && conversation.VendorId != currentUser.VendorId.Value)
            {
                return _responseService.ErrorResponse<GetConversationDTO>("You do not have access to this conversation");
            }

            // 3) Project to DTO
            var dto = new GetConversationDTO
            {
                Id               = conversation.Id,
                LastMessageTime  = conversation.LastMessageTime,
                BuyerId          = conversation.BuyerId,
                BuyerName        = $"{conversation.Buyer.Profile.FirstName} {conversation.Buyer.Profile.LastName}",
                BuyerImageUrl    = conversation.Buyer.Profile.ProfilePictureUrl,
                VendorId         = conversation.VendorId,
                VendorName       = conversation.Vendor.BrandName,
                VendorImageUrl   = conversation.Vendor.VendorPictureUrl,
                Messages         = conversation.Messages
                                     .OrderBy(m => m.Created)
                                     .Select(m => new GetMessageDTO
                                     {
                                         Id           = m.Id,
                                         Content      = m.Content,
                                         SentAt       = m.Created,
                                         SenderId     = m.SenderId,
                                         SenderName   = m.IsFromBuyer 
                                                         ? $"{conversation.Buyer.Profile.FirstName} {conversation.Buyer.Profile.LastName}"
                                                         : conversation.Vendor.BrandName,
                                         IsFromBuyer  = m.IsFromBuyer,
                                         Status       = m.Status.ToString()
                                     })
                                     .ToList()
            };

            if (conversation.Ads != null)
            {
                dto.Ad = new GetAdsDTO
                {
                    Id          = conversation.Ads.Id,
                    Title       = conversation.Ads.Title,
                    Description = conversation.Ads.Description,
                    Price       = conversation.Ads.Price
                };
            }

            // 4) Mark unread messages as read
            await MarkMessagesAsReadInternalAsync(conversation, currentUser);

            return _responseService.SuccessResponse(dto);
        }

        public async Task<ServiceResponse<List<ConversationSummaryDTO>>> GetUserConversationsAsync(UserDTO currentUser)
        {
            // 1) Base query
            var query = _unitOfWork.Context.Conversations
                .AsNoTracking()
                .Include(c => c.Buyer).ThenInclude(b => b.Profile)
                .Include(c => c.Vendor)
                .Include(c => c.Ads)
                .Include(c => c.Messages)
                .AsQueryable();

            // 2) Filter by buyer or vendor
            if (currentUser.BuyerId.HasValue)
                query = query.Where(c => c.BuyerId == currentUser.BuyerId.Value);
            else if (currentUser.VendorId.HasValue)
                query = query.Where(c => c.VendorId == currentUser.VendorId.Value);
            else
                return _responseService.ErrorResponse<List<ConversationSummaryDTO>>("User profile not found");

            // 3) Execute and project
            var conversations = await query
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();

            var results = conversations.Select(c =>
            {
                var lastMsg = c.Messages.OrderByDescending(m => m.Created).FirstOrDefault();
                int unread = currentUser.BuyerId.HasValue
                    ? c.Messages.Count(m => !m.IsFromBuyer && m.Status != MessageStatus.Read)
                    : c.Messages.Count(m => m.IsFromBuyer  && m.Status != MessageStatus.Read);

                return new ConversationSummaryDTO
                {
                    Id              = c.Id,
                    LastMessageTime = c.LastMessageTime,
                    LastMessage     = lastMsg?.Content,
                    BuyerId         = c.BuyerId,
                    BuyerName       = $"{c.Buyer.Profile.FirstName} {c.Buyer.Profile.LastName}",
                    BuyerImageUrl   = c.Buyer.Profile.ProfilePictureUrl,
                    VendorId        = c.VendorId,
                    VendorName      = c.Vendor.BrandName,
                    VendorImageUrl  = c.Vendor.VendorPictureUrl,
                    AdsId           = c.AdsId,
                    AdsTitle        = c.Ads?.Title,
                    UnreadCount     = unread
                };
            }).ToList();

            return _responseService.SuccessResponse(results);
        }

        public async Task<ServiceResponse<string>> MarkMessagesAsReadAsync(Guid conversationId, UserDTO currentUser)
        {
            // 1) Load conversation
            var conversation = await _unitOfWork.Context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return _responseService.ErrorResponse<string>("Conversation not found");

            // 2) Ensure participation
            if (currentUser.BuyerId.HasValue && conversation.BuyerId != currentUser.BuyerId.Value &&
                currentUser.VendorId.HasValue && conversation.VendorId != currentUser.VendorId.Value)
            {
                return _responseService.ErrorResponse<string>("You do not have access to this conversation");
            }

            // 3) Mark as read
            await MarkMessagesAsReadInternalAsync(conversation, currentUser);
            return _responseService.SuccessResponse("Messages marked as read");
        }

        public async Task<ServiceResponse<GetMessageDTO>> SendMessageAsync(SendMessageDTO model, UserDTO currentUser)
        {
            // 1) Validate content
            if (string.IsNullOrWhiteSpace(model.Content))
                return _responseService.ErrorResponse<GetMessageDTO>("Message content cannot be empty");

            Conversation conversation;
            bool isNew = false;
            // 2) Determine existing vs new conversation
            if (model.ConversationId.HasValue)
            {
                conversation = await _unitOfWork.Context.Conversations
                    .Include(c => c.Buyer).ThenInclude(b => b.Profile)
                    .Include(c => c.Vendor)
                    .FirstOrDefaultAsync(c => c.Id == model.ConversationId.Value);

                if (conversation == null)
                    return _responseService.ErrorResponse<GetMessageDTO>("Conversation not found");

                if (currentUser.BuyerId.HasValue && conversation.BuyerId != currentUser.BuyerId.Value &&
                    currentUser.VendorId.HasValue && conversation.VendorId != currentUser.VendorId.Value)
                {
                    return _responseService.ErrorResponse<GetMessageDTO>("You do not have access to this conversation");
                }
            }
            else
            {
                // 2a) Starting new conversation
                if (!model.VendorId.HasValue)
                    return _responseService.ErrorResponse<GetMessageDTO>("Vendor ID is required to start a new conversation");
                if (currentUser.BuyerId == null)
                    return _responseService.ErrorResponse<GetMessageDTO>("You must be logged in as a buyer to start a conversation");

                var vendorExists = await _unitOfWork.Context.Set<Vendor>()
                    .AsNoTracking()
                    .AnyAsync(v => v.Id == model.VendorId.Value);
                if (!vendorExists)
                    return _responseService.ErrorResponse<GetMessageDTO>("Vendor not found");

                Ads ad = null;
                if (model.AdsId.HasValue)
                {
                    ad = await _unitOfWork.Context.Ads.FindAsync(model.AdsId.Value);
                    if (ad == null)
                        return _responseService.ErrorResponse<GetMessageDTO>("Ad not found");
                    if (ad.VendorId != model.VendorId.Value)
                        return _responseService.ErrorResponse<GetMessageDTO>("The specified ad does not belong to this vendor");
                }

                var existing = await _unitOfWork.Context.Set<Conversation>()
                    .FirstOrDefaultAsync(c =>
                        c.BuyerId == currentUser.BuyerId.Value &&
                        c.VendorId == model.VendorId.Value &&
                        (model.AdsId == null || c.AdsId == model.AdsId.Value));

                if (existing != null)
                {
                    conversation = existing;
                }
                else
                {
                    conversation = new Conversation
                    {
                        BuyerId         = currentUser.BuyerId.Value,
                        VendorId        = model.VendorId.Value,
                        AdsId           = model.AdsId,
                        LastMessageTime = DateTimeOffset.UtcNow
                    };
                    await _unitOfWork.Context.Conversations.AddAsync(conversation);
                    isNew = true;
                }
                await _unitOfWork.CommitAsync();
            }

            // 3) Create and save message
            var message = new Message
            {
                Content        = model.Content,
                SenderId       = currentUser.Id,
                IsFromBuyer    = currentUser.BuyerId.HasValue,
                ConversationId = conversation.Id,
                Status         = MessageStatus.Sent
            };

            conversation.LastMessageTime = DateTimeOffset.UtcNow;
            await _unitOfWork.Context.Messages.AddAsync(message);
            await _unitOfWork.CommitAsync();

            // 4) Project to DTO
            var result = new GetMessageDTO
            {
                Id         = message.Id,
                Content    = message.Content,
                SentAt     = message.Created,
                SenderId   = message.SenderId,
                SenderName = message.IsFromBuyer
                    ? $"{conversation.Buyer.Profile.FirstName} {conversation.Buyer.Profile.LastName}"
                    : conversation.Vendor.BrandName,
                IsFromBuyer = message.IsFromBuyer,
                Status      = message.Status.ToString()
            };

            return _responseService.SuccessResponse(result);
        }

        // Marks unread messages as read and commits
        private async Task MarkMessagesAsReadInternalAsync(Conversation conversation, UserDTO currentUser)
        {
            var toMark = conversation.Messages.Where(m =>
                currentUser.BuyerId.HasValue
                    ? (!m.IsFromBuyer && m.Status != MessageStatus.Read)
                    : ( m.IsFromBuyer && m.Status != MessageStatus.Read));

            if (!toMark.Any()) return;

            foreach (var msg in toMark)
                msg.Status = MessageStatus.Read;

            await _unitOfWork.CommitAsync();
        }
    }
}
