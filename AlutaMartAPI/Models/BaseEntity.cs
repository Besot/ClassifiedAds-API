using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlutaMartAPI.Models;

    public abstract class BaseEntity
    {
        protected BaseEntity()
    {
        Created = DateTime.UtcNow;
        Modified = Created;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Modified { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public bool IsDeleted { get; set; }   
    }