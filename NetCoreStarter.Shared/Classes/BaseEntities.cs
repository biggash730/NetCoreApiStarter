using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreStarter.Shared.Classes
{
    public class HasId
    {
        public long Id { get; set; }
    }

    public class AuditFields : HasId
    {
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public string ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool Locked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }

    public class LookUp : AuditFields
    {
        [MaxLength(512), Required, Index(IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
    }

}
