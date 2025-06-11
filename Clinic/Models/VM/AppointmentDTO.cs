using Clinic.Models.Catalogs;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.VM
{
    public class AppointmentIndex
    {
        public Guid AppointmentId { get; set; }
        
        [Display(Name = "Estado")]
        public AppointmentStatus AppointmentStatus { get; set; }
        
        [Display(Name = "Código")]
        public string TrackingId { get; set; } = string.Empty;

        [Display(Name = "Fecha y hora de la cita")]
        public DateTime AppointmentDateTime { get; set; }
        
        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; } = string.Empty;
        
        [Display(Name = "Nombre (s)")]
        public string DoctorName { get; set; } = string.Empty;
        
        [Display(Name = "Apellido paterno")]
        public string DoctorPaternalSurname { get; set; } = string.Empty;
        
        [Display(Name = "Apellido materno")]
        public string DoctorMaternalSurname { get; set; } = string.Empty;

        [Display(Name = "Razón de la visita")]
        public string ReasonForVisit { get; set; } = string.Empty;

        [Display(Name = "Doctor")]
        public string DoctorFullName => $"{DoctorPaternalSurname} {DoctorMaternalSurname}, {DoctorName}";
    }

    public class AppointmentDetails : AppointmentIndex
    {
        [Display(Name = "Notas administrativas")]
        public string AdministrativeNotes { get; set; } = string.Empty;

        [Display(Name = "Nombre (s)")]
        public string ClientName { get; set; } = string.Empty;
        [Display(Name = "Apellido paterno")]
        public string ClientPaternalSurname { get; set; } = string.Empty;
        [Display(Name = "Apellido materno")]
        public string ClientMaternalSurname { get; set; } = string.Empty;
        [Display(Name = "Paciente")]
        public string ClientFullName => $"{ClientPaternalSurname} {ClientMaternalSurname}, {ClientName}";
    }

    public class AppointmentCreate
    {
        [Required]
        [Display(Name = "Razón de la visita")]
        public string ReasonForVisit { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Especialidad")]
        public Guid SpecialtyId { get; set; }
        [Required]
        [Display(Name = "Doctor")]
        public Guid DoctorId { get; set; }
        [Required]
        [Display(Name = "Fecha y hora de la cita")]
        public Guid SlotId { get; set; }
    }
}
