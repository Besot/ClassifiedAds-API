using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlutaMartAPI.Services
{
    public interface IMessageService
    {
        Task<ServiceResponse<GetMessageDTO>> SendMessageAsync(SendMessageDTO model, UserDTO currentUser);
        Task<ServiceResponse<GetConversationDTO>> GetConversationAsync(Guid conversationId, UserDTO currentUser);
        Task<ServiceResponse<List<ConversationSummaryDTO>>> GetUserConversationsAsync(UserDTO currentUser);
        Task<ServiceResponse<string>> MarkMessagesAsReadAsync(Guid conversationId, UserDTO currentUser);
    }
}
