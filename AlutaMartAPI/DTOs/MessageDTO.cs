using AlutaMartAPI.Models;
using System;
using System.Collections.Generic;

namespace AlutaMartAPI.DTOs
{
    public class SendMessageDTO
    {
        public string Content { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? AdsId { get; set; }
    }

    public class GetMessageDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public bool IsFromBuyer { get; set; }
        public string Status { get; set; }
    }

    public class GetConversationDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerImageUrl { get; set; }
        
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorImageUrl { get; set; }
        
        public GetAdsDTO Ad { get; set; }
        
        public List<GetMessageDTO> Messages { get; set; } = new List<GetMessageDTO>();
    }

    public class ConversationSummaryDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastMessageTime { get; set; }
        public string LastMessage { get; set; }
        
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerImageUrl { get; set; }
        
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorImageUrl { get; set; }
        
        public Guid? AdsId { get; set; }
        public string AdsTitle { get; set; }
        
        public int UnreadCount { get; set; }
    }
}
