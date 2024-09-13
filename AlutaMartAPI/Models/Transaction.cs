using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

public class Transaction : BaseEntity
{
    public double Amount { get; set; }
    public double Charges { get; set; }
    public double Revenue { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentType PaymentType { get; set; }
    public string Narration { get; set; }

    public DateTimeOffset DatePaid { get; set; }    
    public double Reference { get; set; }
    public string ExternalReference { get; set; }

    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }

    public Guid? PaymentInflowId { get; set; }
    public virtual PaymentInflow PaymentInflow { get; set; }

    public Guid? PaymentOutflowId { get; set; }
    public virtual PaymentOutflow PaymentOutflow { get; set; }
}