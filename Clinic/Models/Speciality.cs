using Clinic.Models.Core;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    /// <summary>
    /// Represents a medical specialty.
    /// </summary>
    public class Specialty : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Especialidad")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public Catalogs.StatusCore Status { get; set; } 

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        [Display(Name = "Clave")]
        public string Code { get; set; } = string.Empty; // Unique code for the specialty, e.g., "11" for Cardiology

        // Navigation property: A specialty can have many doctors.
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
