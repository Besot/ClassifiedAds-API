namespace AlutaMartAPI.DTOs;
    public class GetVendorReviewDTO
    {
        public Guid ReviewId { get; set; }
        public string Review { get; set; }
        public string ReviewerName { get; set; }
        public DateTimeOffset ReviewDate { get; set; }

    }