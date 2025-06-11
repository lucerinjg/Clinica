using Clinic.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    /// <summary>
    /// Represents a patient of the clinic.
    /// Ideally, this class would be containt additional properties for a patient's details,
    /// </summary>
    public class Patient
    {
        [Key]
        public Guid ApplicationUserId { get; set; }

        [Display(Name = "Código")]
        public ulong Code { get; set; }

        // Navigation property: A patient can have many appointments.
        public virtual ICollection<Appointment> Appointments { get; set; } = [];

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }

}
