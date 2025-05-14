namespace AlutaMartAPI.Models;

public class Review : BaseEntity
{
    public string Content { get; set; }
    public int Rating { get; set; }
    public Guid? VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }
    public Guid? AdsId { get; set; }
    public virtual Ads Ads { get; set; }
    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }
}