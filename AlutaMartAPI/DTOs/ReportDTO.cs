using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using System;

namespace AlutaMartAPI.DTOs
{
    public class CreateReportDTO
    {
        public string Description { get; set; }
        public ReportType Type { get; set; }
        public Guid? AdsId { get; set; }
        public Guid? VendorId { get; set; }
    }

    public class GetReportDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string AdminNote { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string ReporterName { get; set; }
        
        // Reported content
        public GetAdsDTO ReportedAd { get; set; }
        public GetVendorDTO ReportedVendor { get; set; }
    }

    public class UpdateReportStatusDTO
    {
        public ReportStatus Status { get; set; }
        public string AdminNote { get; set; }
    }
}
