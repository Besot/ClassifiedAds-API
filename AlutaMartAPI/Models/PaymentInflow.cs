using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

public class PaymentInflow : BaseEntity
{
    public double Amount { get; set; }
    public double Charges { get; set; }
    public double Revenue { get; set; }

    public DateTimeOffset? DatePaid { get; set; }    
    public double Reference { get; set; }

    public string ExternalReference { get; set; }
    public string Narration { get; set; }

    public PaymentStatus Status { get; set; }
    public PaymentProcessor Processor { get; set; }


    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    public Guid? AdId { get; set; }
    public virtual Ads Ads { get; set; }

    public Guid? VendorId { get; set; }
    public virtual Vendor Vendor { get; set; }

    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }
}