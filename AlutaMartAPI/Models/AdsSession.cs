namespace AlutaMartAPI.Models;

public class AdsSession : BaseEntity
{
    public string ContentVideoLink { get; set; }
    public Guid VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid AdsId { get; set; }
    public virtual Ads Ads { get; set; }

}