using AlutaMartAPI.DTOs;
using AlutaMartAPI.Hubs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlutaMartAPI.Controllers
{
    /// <summary>
    /// Message controller for handling messaging operations.
    /// Note: For real-time messaging, use the SignalR ChatHub at /chatHub instead of these REST endpoints.
    /// </summary>
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        
        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Legacy API endpoint to send a message. For real-time chat, use the SignalR ChatHub instead.
        /// </summary>
        [HttpPost("Send"), BlockAccess(Roles.SuperAdmin)]
        [ProducesResponseType(typeof(ServiceResponse<GetMessageDTO>), 200)]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO model)
        {
            // Send the message using the service
            var result = await _messageService.SendMessageAsync(model, CurrentUser);
            
            // Skip SignalR notification if we don't have message data
            if (result == null || result.Data == null)
                return Ok(result);
            
            // For new conversations or when there's no ConversationId in the request,
            // we need to extract conversation details from the message service
            if (!model.ConversationId.HasValue)
            {
                // Query all user conversations to find where this message belongs
                var userConversations = await _messageService.GetUserConversationsAsync(CurrentUser);
                if (userConversations?.Data != null && userConversations.Data.Count > 0)
                {
                    // Get the most recent conversation (likely the one we just created)
                    var latestConversation = userConversations.Data
                        .OrderByDescending(c => c.LastMessageTime)
                        .FirstOrDefault();
                    
                    if (latestConversation != null)
                    {
                        // Determine recipient based on user role
                        var recipientId = CurrentUser.BuyerId.HasValue ?
                            latestConversation.VendorId.ToString() : latestConversation.BuyerId.ToString();
                        
                        // Notify recipient using SignalR
                        await _hubContext.Clients.Group($"user_{recipientId}")
                            .SendAsync("ReceiveMessage", result.Data);
                    }
                }
            }
            else
            {
                // For existing conversations, get the conversation details
                var conversation = await _messageService.GetConversationAsync(model.ConversationId.Value, CurrentUser);
                
                // Only send the SignalR notification if we successfully retrieved the conversation
                if (conversation != null && conversation.Data != null)
                {
                    var recipientId = CurrentUser.BuyerId.HasValue ?
                        conversation.Data.VendorId.ToString() : conversation.Data.BuyerId.ToString();
                    
                    // Notify recipient using SignalR
                    await _hubContext.Clients.Group($"user_{recipientId}")
                        .SendAsync("ReceiveMessage", result.Data);
                }
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Gets a conversation by ID. Real-time updates are available through SignalR.
        /// </summary>
        [HttpGet("Conversation/{conversationId}"), BlockAccess(Roles.SuperAdmin)]
        [ProducesResponseType(typeof(ServiceResponse<GetConversationDTO>), 200)]
        public async Task<IActionResult> GetConversation(Guid conversationId)
        {
            return Ok(await _messageService.GetConversationAsync(conversationId, CurrentUser));
        }

        /// <summary>
        /// Gets all conversations for the current user. For real-time updates, connect to SignalR.
        /// </summary>
        [HttpGet("MyConversations"), BlockAccess(Roles.SuperAdmin)]
        [ProducesResponseType(typeof(ServiceResponse<List<ConversationSummaryDTO>>), 200)]
        public async Task<IActionResult> GetUserConversations()
        {
            return Ok(await _messageService.GetUserConversationsAsync(CurrentUser));
        }

        /// <summary>
        /// Marks all messages in a conversation as read. For real-time notifications, use SignalR.
        /// </summary>
        [HttpPut("MarkAsRead/{conversationId}"), BlockAccess(Roles.SuperAdmin)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> MarkMessagesAsRead(Guid conversationId)
        {
            var result = await _messageService.MarkMessagesAsReadAsync(conversationId, CurrentUser);
            
            // Only send SignalR notification if the operation completed successfully and returned data
            if (result != null && result.Data != null)
            {
                // Get the conversation to notify the other participant
                var conversation = await _messageService.GetConversationAsync(conversationId, CurrentUser);
                if (conversation != null && conversation.Data != null)
                {
                    // Determine the other participant
                    var otherParticipantId = CurrentUser.BuyerId.HasValue ?
                        conversation.Data.VendorId.ToString() : conversation.Data.BuyerId.ToString();
                    
                    // Notify through SignalR that messages have been read
                    await _hubContext.Clients.Group($"user_{otherParticipantId}")
                        .SendAsync("MessagesRead", conversationId);
                }
            }
            
            return Ok(result);
        }
    }
}
