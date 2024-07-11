using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
public class Ads :BaseEntity
    {
    public string Title { get; set; }
    public double Amount { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public string AdsImageUrl { get; set; }
    public Guid BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public AdsType AdsType { get; set; }
    public AdsCondition AdsCondition { get; set; }
    
    public Guid VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid AdsCategoryId { get; set; }
    public virtual AdsCategory AdsCategory { get; set; }

    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    }