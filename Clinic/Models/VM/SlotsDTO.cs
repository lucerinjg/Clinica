using Clinic.Models.Catalogs;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.VM
{
    public class SlotsCreate
    {
        [Display(Name = "Fecha de inicio")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Fecha de finalización de consultas")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Duración de consultas")]
        [Range(10,30)]
        public int DurationInMinutes { get; set; }
    }

    public class SlotsEdit
    {
        public Guid SlotId { get; set; }
        
        [Display(Name = "Estado")]
        public StatusCore Status { get; set; }
    }

    public class SlotsIndex
    {
        public Guid SlotId { get; set; }

        /// <summary>
        /// Unique identifier for the doctor
        /// </summary>
        public Guid DoctorId { get; set; }
        
        ///// <summary>
        ///// Specialty of the doctor
        ///// </summary>
        //[Display(Name = "Especialidad")]
        //public string SpecialtyName { get; set; } = string.Empty;
        
        /// <summary>
        /// Name of the doctor
        /// </summary>
        [Display(Name = "Nombre (s)")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Apellido paterno")]
        public string PaternalSurname { get; set; } = string.Empty;
        
        [Display(Name = "Apellido materno")]
        public string MaternalSurname { get; set; } = string.Empty;

        [Display(Name = "Nombre Dr.")]
        public string FullName => $"{PaternalSurname} {MaternalSurname}, {Name}";    

        [Display(Name = "Reservado")]
        public bool IsBooked { get; set; } = false;

        [Display(Name = "Fecha de inicio")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Fecha de término")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Estado")]
        public StatusCore Status { get; set; }
    }
}
