using Microsoft.EntityFrameworkCore.Metadata;

namespace AlutaMartAPI.Models;
    public class PlanTier : BaseEntity
    {
    public string Name { get; set; }
    public double Amount { get; set; }
    public int MaxAds { get; set; }
    public int MaxPicture { get; set; }
    public bool IsFeatured { get; set; }
    }