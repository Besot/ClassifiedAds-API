namespace AlutaMartAPI.DTOs;
    public class GetPlanTierDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int MaxAds { get; set; }
        public int MaxPicture { get; set; }
        public int MaxFeatured { get; set; }
    }