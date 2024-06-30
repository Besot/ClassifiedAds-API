namespace AlutaMartAPI.Models;
    public class AdsReceipt : BaseEntity
    {
    public string ReceiptUrl { get; set; }
    public Guid AdsId { get; set; }
    public virtual Ads Ads { get; set; }
    }