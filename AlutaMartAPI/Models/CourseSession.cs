namespace AlutaMartAPI.Models;

public class CourseSession : BaseEntity
{
    public string ContentVideoLink { get; set; }
    public Guid ExpertId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid AdsId { get; set; }
    public virtual Ads Ads { get; set; }

}