using Clinic.Models.Catalogs;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.VM
{
    public class DoctorCreate
    {
        /// <summary>
        /// Name of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Nombre (s)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Paternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Apellido paterno")]
        public string PaternalSurname { get; set; } = string.Empty;

        /// <summary>
        /// Maternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Apellido materno")]
        public string MaternalSurname { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} carateres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Especialidad")]
        public Guid SpecialtyId { get; set; }
    }

    public class DoctorIndex : DoctorCreate
    {
        /// <summary>
        /// Unique identifier for the doctor
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Specialty name of the doctor
        /// </summary>
        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; } = string.Empty;

        [Display(Name = "F. Registro")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Estado")]
        public StatusCore Status { get; set; } = StatusCore.Active;
    }

    public class DoctorEdit
    {
        public Guid Id { get; set; }

        [Display(Name = "Estado")]
        public StatusCore Status { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Nombre (s)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Paternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Apellido paterno")]
        public string PaternalSurname { get; set; } = string.Empty;

        /// <summary>
        /// Maternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        [Display(Name = "Apellido materno")]
        public string MaternalSurname { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Especialidad")]
        public Guid SpecialtyId { get; set; }
    }
}