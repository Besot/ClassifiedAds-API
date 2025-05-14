
namespace AlutaMartAPI.Models;

public class BankAccount : BaseEntity
{
    public Guid ProfileId { get; set; }
    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }
    public string BankAccountName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankName { get; set; }
    public bool IsDefault { get; set; } = false;
#nullable enable
    public string? BankCode { get; set; } = string.Empty;
}
