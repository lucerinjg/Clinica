using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.Catalogs
{
    /// <summary>
    /// Enum class for common status on db
    /// </summary>
    public enum StatusCore
    {
        [Display(Name = "Activo")]
        Active,
        [Display(Name = "Inactivo")]
        Inactive,
    }
}
