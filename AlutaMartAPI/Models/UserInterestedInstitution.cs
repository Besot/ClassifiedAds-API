namespace AlutaMartAPI.Models;
    public class UserInterestedInstitution : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile {get; set; }
        public Guid InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
    }