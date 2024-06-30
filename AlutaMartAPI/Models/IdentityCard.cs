namespace AlutaMartAPI.Models;
    public class IdentityCard : BaseEntity
    {
    public string CertificateLinkUrl { get; set; }

    public Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; }  
    }