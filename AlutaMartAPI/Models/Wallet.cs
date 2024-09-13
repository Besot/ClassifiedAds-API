namespace AlutaMartAPI.Models;

public class Wallet : BaseEntity
{
    public double Amount { get; set; }

    public Guid CurrencyId { get; set; }
    public virtual Currency Currency { get; set; }

    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }
}