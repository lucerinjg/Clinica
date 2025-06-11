using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.Core
{
    /// <summary>
    /// Define interface for base models
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Define key property for entities
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Define creation date of object
        /// </summary>
        DateTime CreationDate { get; set; }
    }

    public abstract class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Date time for creation
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
