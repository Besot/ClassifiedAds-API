using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
    public class Buyer : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        
        public int CartCount { get; set; }
    }
