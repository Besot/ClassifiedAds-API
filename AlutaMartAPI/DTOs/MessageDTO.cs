using AlutaMartAPI.Models;
using System;
using System.Collections.Generic;

namespace AlutaMartAPI.DTOs
{
    public class SendMessageDTO
    {
        public string Content { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? VendorId { get; set; } // This will be the recipient vendor's ID if initiating a new chat
    }

    public class GetMessageDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public Guid SenderProfileId { get; set; }
        public string SenderName { get; set; }
        public bool IsFromBuyer { get; set; } // True if sender is buyer, false if sender is vendor
        public string Status { get; set; } // e.g., "sent", "delivered", "read"
    }

    public class GetConversationDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        
        public Guid BuyerProfileId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerImageUrl { get; set; }
        
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorImageUrl { get; set; }
        
        // The Ad property is removed as conversations are now strictly between buyer and vendor
        
        public List<GetMessageDTO> Messages { get; set; } = new List<GetMessageDTO>();
    }

    public class ConversationSummaryDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        public string LastMessage { get; set; }
        
        public Guid BuyerProfileId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerImageUrl { get; set; }
        
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorImageUrl { get; set; }
        
        // AdsId and AdsTitle are removed as conversations are now strictly between buyer and vendor
        
        public int UnreadCount { get; set; }
    }
}