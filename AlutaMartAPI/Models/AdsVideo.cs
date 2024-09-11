namespace AlutaMartAPI.Models;

public class AdsVideo : BaseEntity
{
    public string AdVideoLink { get; set; }
    public Guid VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid AdId { get; set; }
    public virtual Ads Ads { get; set; }

}