using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
    public class Report : BaseEntity
    {
        public string Description { get; set; }
        public ReportType Type { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public string AdminNote { get; set; }
        
        // Reporter
        public Guid ReporterId { get; set; }
        public Guid? BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
        
        // Reported content
        public Guid? AdsId { get; set; }
        public virtual Ads Ads { get; set; }
        public Guid? VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
