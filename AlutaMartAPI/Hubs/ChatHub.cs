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
                VendorId = GetNullableGuid(Context.User.FindFirst("vendorId")?.Value),
                Access = (Roles)Enum.Parse(typeof(Roles), Context.User.FindFirst("role")?.Value ?? "Guest")
            };

            var model = new SendMessageDTO
            {
                ConversationId = conversationId,
                Content = message
                // AdsId is removed here as it's no longer relevant for direct buyer-vendor chats
            };

            var result = await _messageService.SendMessageAsync(model, currentUser);

            if (result != null)
            {
                // Get the conversation participants
                var conversation = await _messageService.GetConversationAsync(conversationId, currentUser);
                if (conversation != null)
                {
                    // Determine the recipient ID based on who the current user is
                    Guid recipientProfileId;
                    if (currentUser.Access == Roles.Buyer) // Current user is a buyer
                    {
                        recipientProfileId = conversation.Data.VendorId; 
                    }
                    else if (currentUser.VendorId.HasValue) // Current user is a vendor
                    {
                        recipientProfileId = conversation.Data.BuyerProfileId;
                    }
                    else // This case shouldn't ideally happen with BuyerId/VendorId checks
                    {
                        return; 
                    }
                    
                    // Send to specific user group
                    await Clients.Group($"user_{recipientProfileId}").SendAsync("ReceiveMessage", result.Data);
                    
                    // Also send to buyer or vendor group if applicable, ensuring the correct group receives the message
                    if (currentUser.Access == Roles.Buyer)
                    {
                        await Clients.Group($"vendor_{conversation.Data.VendorId}").SendAsync("ReceiveMessage", result.Data);
                    }
                    else if (currentUser.VendorId.HasValue)
                    {
                        await Clients.Group($"buyer_{conversation.Data.BuyerProfileId}").SendAsync("ReceiveMessage", result.Data);
                    }
                }
            }
        }

        // Added method for initiating a new conversation directly with a vendor
        public async Task StartNewConversationWithVendor(Guid vendorId, string initialMessage)
        {
            var currentUser = new UserDTO
            {
                Id = Guid.Parse(Context.User.FindFirst("id")?.Value),
                Access = (Roles)Enum.Parse(typeof(Roles), Context.User.FindFirst("role")?.Value ?? "Guest")
            };

            // Ensure only buyers can initiate a new conversation with a vendor this way
            if (currentUser.Access != Roles.Buyer)
            {
                // Optionally send an error back to the client
                await Clients.Caller.SendAsync("ReceiveError", "Only buyers can start new conversations with vendors.");
                return;
            }

            var model = new SendMessageDTO
            {
                VendorId = vendorId, // Specify the vendor to start the conversation with
                Content = initialMessage
            };

            var result = await _messageService.SendMessageAsync(model, currentUser);

            if (result != null && result.Data != null)
            {
                // Notify the sender that the conversation has been created/message sent
                await Clients.Caller.SendAsync("MessageSentConfirmation", result.Data);

                // Notify the vendor of the new message/conversation
                await Clients.Group($"user_{vendorId}").SendAsync("ReceiveMessage", result.Data);
                await Clients.Group($"vendor_{vendorId}").SendAsync("ReceiveMessage", result.Data);
            }
            else if (result != null && !string.IsNullOrEmpty(result.Message))
            {
                await Clients.Caller.SendAsync("ReceiveError", result.Message);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveError", "Failed to send message.");
            }
        }


        public async Task MarkConversationAsRead(Guid conversationId)
        {
            var currentUser = new UserDTO
            {
                Id = Guid.Parse(Context.User.FindFirst("id")?.Value),
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
                    Guid otherParticipantProfileId;
                    if (currentUser.Access == Roles.Buyer) // Current user is buyer, other participant is vendor
                    {
                        otherParticipantProfileId = conversation.Data.VendorId; 
                    }
                    else if (currentUser.VendorId.HasValue) // Current user is vendor, other participant is buyer
                    {
                        otherParticipantProfileId = conversation.Data.BuyerProfileId;
                    }
                    else
                    {
                        return; 
                    }
                    
                    await Clients.Group($"user_{otherParticipantProfileId}").SendAsync("MessagesRead", conversationId);
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