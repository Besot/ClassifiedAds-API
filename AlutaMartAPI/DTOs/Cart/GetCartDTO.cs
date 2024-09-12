namespace AlutaMartAPI.DTOs;
    public class GetCartDTO
    {
        public Guid CartId { get; set; }
        public Guid AdsId { get; set; }
        public string AdsTitle { get; set; }
        public int Quantity { get; set; }
        public double PriceAtTimeOfAdd { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
    }