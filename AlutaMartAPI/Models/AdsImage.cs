namespace AlutaMartAPI.Models;
    public class AdsImage : BaseEntity
    {
    public string ImageUrl { get; set; }

    public Guid AdsId { get; set; }
    public virtual Ads Ads { get; set; }
}