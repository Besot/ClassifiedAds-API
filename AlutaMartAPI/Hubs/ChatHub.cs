using System;
using System.Threading.Tasks;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AlutaMartAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Add(userId, Context.ConnectionId);
                
                // Set user as online in user group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Also join buyer or vendor specific groups if applicable
                string buyerId = Context.User.FindFirst("buyerId")?.Value;
                string vendorId = Context.User.FindFirst("vendorId")?.Value;
                
                if (!string.IsNullOrEmpty(buyerId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"buyer_{buyerId}");
                }
                
                if (!string.IsNullOrEmpty(vendorId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"vendor_{vendorId}");
                }
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Remove(userId, Context.ConnectionId);
                
                // Remove from user group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Also remove from buyer or vendor specific groups if applicable
                string buyerId = Context.User.FindFirst("buyerId")?.Value;
                string vendorId = Context.User.FindFirst("vendorId")?.Value;
                
                if (!string.IsNullOrEmpty(buyerId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"buyer_{buyerId}");
                }
                
                if (!string.IsNullOrEmpty(vendorId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vendor_{vendorId}");
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToConversation(Guid conversationId, string message)
        {
            var currentUser = new UserDTO
            {
                Id = Guid.Parse(Context.User.FindFirst("id")?.Value),
                BuyerId = GetNullableGuid(Context.User.FindFirst("buyerId")?.Value),
                VendorId = GetNullableGuid(Context.User.FindFirst("vendorId")?.Value),
                Access = (Roles)Enum.Parse(typeof(Roles), Context.User.FindFirst("role")?.Value ?? "Guest")
            };

            var model = new SendMessageDTO
            {
                ConversationId = conversationId,
                Content = message
            };

            var result = await _messageService.SendMessageAsync(model, currentUser);

            if (result != null)
            {
                // Get the conversation participants
                var conversation = await _messageService.GetConversationAsync(conversationId, currentUser);
                if (conversation != null)
                {
                    var recipientId = currentUser.BuyerId.HasValue ? 
                        conversation.Data.VendorId : conversation.Data.BuyerId;
                    
                    // Send to specific user group
                    await Clients.Group($"user_{recipientId}").SendAsync("ReceiveMessage", result.Data);
                    
                    // Also send to buyer or vendor group if applicable
                    if (currentUser.BuyerId.HasValue)
                    {
                        await Clients.Group($"vendor_{conversation.Data.VendorId}").SendAsync("ReceiveMessage", result.Data);
                    }
                    else if (currentUser.VendorId.HasValue)
                    {
                        await Clients.Group($"buyer_{conversation.Data.BuyerId}").SendAsync("ReceiveMessage", result.Data);
                    }
                }
            }
        }

        public async Task MarkConversationAsRead(Guid conversationId)
        {
            var currentUser = new UserDTO
            {
                Id = Guid.Parse(Context.User.FindFirst("id")?.Value),
                BuyerId = GetNullableGuid(Context.User.FindFirst("buyerId")?.Value),
                VendorId = GetNullableGuid(Context.User.FindFirst("vendorId")?.Value),
                Access = (Roles)Enum.Parse(typeof(Roles), Context.User.FindFirst("role")?.Value ?? "Guest")
            };

            var result = await _messageService.MarkMessagesAsReadAsync(conversationId, currentUser);

            if (result != null)
            {
                // Notify the other participant that messages have been read
                var conversation = await _messageService.GetConversationAsync(conversationId, currentUser);
                if (conversation !=null)
                {
                    var otherParticipantId = currentUser.BuyerId.HasValue ? 
                        conversation.Data.VendorId : conversation.Data.BuyerId;
                    
                    await Clients.Group($"user_{otherParticipantId}").SendAsync("MessagesRead", conversationId);
                }
            }
        }

        private Guid? GetNullableGuid(string value)
        {
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            return null;
        }
    }
}
