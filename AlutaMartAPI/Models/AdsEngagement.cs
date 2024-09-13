namespace AlutaMartAPI.Models;

public class AdsEngagement :BaseEntity
{
    public Guid AdId { get; set; }
    public virtual Ads Ads { get; set; }
    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }
    public int VisitCount { get; set; }
    public bool IsEnrolled { get; set; }
}