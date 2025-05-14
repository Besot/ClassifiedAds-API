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
                .Include(b => b.Profile) // Buyer's Profile
                .Include(c => c.Vendor)  
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return _responseService.ErrorResponse<GetConversationDTO>("Conversation not found");

            if (conversation.ProfileId != currentUser.Id &&
                (!currentUser.VendorId.HasValue || conversation.VendorId != currentUser.VendorId.Value))
            {
                return _responseService.ErrorResponse<GetConversationDTO>("You do not have access to this conversation");
            }

            // 3) Project to DTO
            var dto = new GetConversationDTO
            {
                Id               = conversation.Id,
                LastMessageTime  = conversation.LastMessageTime,
                BuyerProfileId    = conversation.ProfileId,
                BuyerName        = $"{conversation.Profile.FirstName} {conversation.Profile.LastName}",
                BuyerImageUrl    = conversation.Profile.ProfilePictureUrl,
                VendorId         = conversation.VendorId, // ExpertId
                VendorName       = conversation.Vendor.BrandName, // Expert's Name
                VendorImageUrl   = conversation.Vendor.VendorPictureUrl, // Expert's Image URL
                Messages         = conversation.Messages
                                     .OrderBy(m => m.Created)
                                     .Select(m => new GetMessageDTO
                                     {
                                         Id           = m.Id,
                                         Content      = m.Content,
                                         SentAt       = m.Created,
                                         SenderProfileId     = m.SenderProfileId,
                                         SenderName   = m.IsFromBuyer 
                                                         ? $"{conversation.Profile.FirstName} {conversation.Profile.LastName}"
                                                         : conversation.Vendor.BrandName,
                                         IsFromBuyer  = m.IsFromBuyer,
                                         Status       = m.Status.ToString()
                                     })
                                     .ToList()
            };

            // Removed: if (conversation.Ads != null) block as ads are no longer part of the conversation.

            // 4) Mark unread messages as read
            await MarkMessagesAsReadInternalAsync(conversation, currentUser);

            return _responseService.SuccessResponse(dto);
        }

        public async Task<ServiceResponse<List<ConversationSummaryDTO>>> GetUserConversationsAsync(UserDTO currentUser)
        {
            // 1) Base query
            var query = _unitOfWork.Context.Conversations
                .AsNoTracking()
                .Include(b => b.Profile) // Buyer's Profile
                .Include(c => c.Vendor)   // Expert's Profile
                .Include(c => c.Messages)
                .AsQueryable();

            // 2) Filter by buyer or expert (vendor)
            if (currentUser.Access == Roles.Buyer)
                query = query.Where(c => c.ProfileId == currentUser.Id);
            else if (currentUser.Access == Roles.Vendor) // Assuming 'Vendor' role now implies 'Expert'
                query = query.Where(c => c.VendorId == currentUser.VendorId.Value);
            else
                return _responseService.ErrorResponse<List<ConversationSummaryDTO>>("User profile not found or invalid role for conversations");

            // 3) Execute and project
            var conversations = await query
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();

            var results = conversations.Select(c =>
            {
                var lastMsg = c.Messages.OrderByDescending(m => m.Created).FirstOrDefault();
                int unread = currentUser.Access == Roles.Buyer
                    ? c.Messages.Count(m => !m.IsFromBuyer && m.Status != MessageStatus.Read)
                    : c.Messages.Count(m => m.IsFromBuyer  && m.Status != MessageStatus.Read);

                return new ConversationSummaryDTO
                {
                    Id              = c.Id,
                    LastMessageTime = c.LastMessageTime,
                    LastMessage     = lastMsg?.Content,
                    BuyerProfileId  = c.ProfileId,
                    BuyerName       = $"{c.Profile.FirstName} {c.Profile.LastName}",
                    BuyerImageUrl   = c.Profile.ProfilePictureUrl,
                    VendorId        = c.VendorId, // ExpertId
                    VendorName      = c.Vendor.BrandName, // Expert's Name
                    VendorImageUrl  = c.Vendor.VendorPictureUrl, // Expert's Image URL
                    // Removed: AdsId and AdsTitle as they are no longer part of conversation summaries
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
            if (conversation.ProfileId != currentUser.Id &&
                (!currentUser.VendorId.HasValue || conversation.VendorId != currentUser.VendorId.Value))
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
            // 2) Determine existing vs new conversation
            if (model.ConversationId.HasValue)
            {
                // Existing conversation logic
                conversation = await _unitOfWork.Context.Conversations
                    .Include(b => b.Profile)
                    .Include(c => c.Vendor)
                    .FirstOrDefaultAsync(c => c.Id == model.ConversationId.Value);

                if (conversation == null)
                    return _responseService.ErrorResponse<GetMessageDTO>("Conversation not found");

                // Ensure the current user is a participant of the found conversation
                if (conversation.ProfileId != currentUser.Id &&
                    (!currentUser.VendorId.HasValue || conversation.VendorId != currentUser.VendorId.Value))
                {
                    return _responseService.ErrorResponse<GetMessageDTO>("You do not have access to this conversation");
                }
            }
            else
            {
                // 2a) Starting new conversation
                // For a buyer to expert conversation, a VendorId (ExpertId) is essential.
                if (!model.VendorId.HasValue)
                    return _responseService.ErrorResponse<GetMessageDTO>("Expert ID (Vendor ID) is required to start a new conversation.");
                
                // Only a buyer can initiate a new conversation with an expert.
                if (currentUser.Access != Roles.Buyer) // Assuming only 'Buyer' can initiate
                    return _responseService.ErrorResponse<GetMessageDTO>("Only buyers can start new conversations with experts.");
                
                // Verify the expert exists
                var expertExists = await _unitOfWork.Context.Vendors // Assuming 'Vendors' table stores experts
                    .AsNoTracking()
                    .AnyAsync(v => v.Id == model.VendorId.Value);
                if (!expertExists)
                    return _responseService.ErrorResponse<GetMessageDTO>("Expert not found.");

                // Removed: Ads related checks as conversations are strictly buyer-expert.

                // Check if a conversation already exists between this buyer and expert
                var existing = await _unitOfWork.Context.Conversations
                    .FirstOrDefaultAsync(c =>
                        c.ProfileId == currentUser.Id &&
                        c.VendorId == model.VendorId.Value);

                if (existing != null)
                {
                    conversation = existing;
                }
                else
                {
                    // Create a new conversation
                    conversation = new Conversation
                    {
                        ProfileId         = currentUser.Id,
                        VendorId        = model.VendorId.Value, // Expert's ID
                        // AdsId is removed here
                        LastMessageTime = DateTimeOffset.UtcNow
                    };
                    await _unitOfWork.Context.Conversations.AddAsync(conversation);
                }
                await _unitOfWork.CommitAsync(); // Commit the conversation creation/retrieval before adding messages
            }

            // 3) Create and save message
            var message = new Message
            {
                Content        = model.Content,
                SenderProfileId       = currentUser.Id,
                IsFromBuyer    = currentUser.Access == Roles.Buyer,
                ConversationId = conversation.Id,
                Status         = MessageStatus.Sent
            };

            conversation.LastMessageTime = DateTimeOffset.UtcNow; // Update last message time on conversation
            await _unitOfWork.Context.Messages.AddAsync(message);
            await _unitOfWork.CommitAsync();

            // 4) Fetch related profiles for DTO projection (if not already loaded by .Include)
            // This ensures SenderName is correctly populated.
            if (conversation.Profile == null && currentUser.Access == Roles.Buyer)
            {
                conversation.Profile = await _unitOfWork.Context.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == currentUser.Id);
            }
            if (conversation.Vendor == null && currentUser.Access == Roles.Vendor)
            {
                conversation.Vendor = await _unitOfWork.Context.Vendors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == currentUser.VendorId.Value);
            }

            // 5) Project to DTO
            var result = new GetMessageDTO
            {
                Id         = message.Id,
                Content    = message.Content,
                SentAt     = message.Created,
                SenderProfileId   = message.SenderProfileId,
                SenderName = message.IsFromBuyer
                    ? $"{conversation.Profile.FirstName} {conversation.Profile.LastName}"
                    : conversation.Vendor.BrandName, // Expert's name
                IsFromBuyer = message.IsFromBuyer,
                Status      = message.Status.ToString()
            };

            return _responseService.SuccessResponse(result);
        }

        // Marks unread messages as read and commits
        private async Task MarkMessagesAsReadInternalAsync(Conversation conversation, UserDTO currentUser)
        {
            var toMark = conversation.Messages.Where(m =>
                currentUser.Access == Roles.Buyer
                    ? (!m.IsFromBuyer && m.Status != MessageStatus.Read) // Buyer marks expert's messages as read
                    : ( m.IsFromBuyer && m.Status != MessageStatus.Read)); // Expert marks buyer's messages as read

            if (!toMark.Any()) return;

            foreach (var msg in toMark)
                msg.Status = MessageStatus.Read;

            await _unitOfWork.CommitAsync();
        }
    }
}