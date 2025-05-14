using System;
using System.Collections.Generic;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models
{


    public class Conversation : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        
        public Guid VendorId { get; set; } 
        public virtual Vendor Vendor { get; set; }
        
        public Guid? AdsId { get; set; }
        public virtual Ads Ads { get; set; }
        
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public DateTimeOffset LastMessageTime { get; set; } = DateTimeOffset.UtcNow;
    }

    public class Message : BaseEntity
    {
        public string Content { get; set; }
        public Guid ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; }
        
        public Guid SenderProfileId { get; set; }
        public virtual Profile SenderProfile { get; set; }
        public bool IsFromBuyer { get; set; }
        
        public MessageStatus Status { get; set; } = MessageStatus.Sent;
    }
}
