using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
    public class AdsDetail : BaseEntity
    {
    public string Info { get; set; }
    public AdsDetailType AdsDetailType { get; set; }

    public Guid CourseId { get; set; }
    public virtual Ads Ads { get; set; }
    }