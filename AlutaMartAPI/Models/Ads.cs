using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
public class Ads :BaseEntity
    {
    public string Title { get; set; }
    public double Amount { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public string CourseImageUrl { get; set; }
    
    public AdsType CourseType { get; set; }
    
    public Guid ExpertId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid AdsCategoryId { get; set; }
    public virtual AdsCategory AdsCategory { get; set; }

    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    }