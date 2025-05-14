
namespace AlutaMartAPI.Models;
    public class SecurityQuestion : BaseEntity
    {
        public Guid ProfileId { get; set; }

        public string SecurityQuestionKey { get; set; }

        public string SecurityQuestionAnswer { get; set; }
    }
