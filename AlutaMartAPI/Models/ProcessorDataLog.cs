using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

public class ProcessorDataLog : BaseEntity
{
    public string ProcessorData { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentProcessor Processor { get; set; }

    public Guid? PaymentInflowId { get; set; }
    public virtual PaymentInflow PaymentInflow { get; set; }

    public Guid? PaymentOutflowId { get; set; }
    public virtual PaymentOutflow PaymentOutflow { get; set; }
}