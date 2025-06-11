using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.Catalogs
{
    public enum AppointmentStatus
    {
        [Display(Name = "Programada")]
        Scheduled,

        [Display(Name = "Completada")]
        Completed,

        [Display(Name = "Cancelada por el paciente")]
        CanceledByPatient,

        [Display(Name = "Cancelada por la clínica")]
        CanceledByClinic,

        [Display(Name = "No Asistió")]
        NoShow
    }
}
