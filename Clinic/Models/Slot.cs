using Clinic.Models.Catalogs;
using Clinic.Models.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class DoctorSlot : BaseEntity
    {
        public StatusCore Status { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; } 

        public bool IsBooked { get; set; } = false;

        // Navigation properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
