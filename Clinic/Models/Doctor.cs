using Clinic.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    /// <summary>
    /// Represents a doctor at the clinic.
    /// </summary>
    public class Doctor
    {
        [Key]
        public Guid ApplicationUserId { get; set; }

        [StringLength(50)]
        public string ProfessionalLicense { get; set; } = string.Empty;

        public Guid SpecialtyId { get; set; }

        // Navigation property to the ApplicationUser, which is the base class for all users in the system.
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Navigation property to the Specialty.
        [ForeignKey("SpecialtyId")]
        public virtual Specialty Specialty { get; set; } = null!;

        // Navigation property: A doctor has a set of available time slots.
        public virtual ICollection<DoctorSlot> AvailableSlots { get; set; } = [];

        // Navigation property: A doctor handles many appointments.
        public virtual ICollection<Appointment> Appointments { get; set; } = [];
    }
}
