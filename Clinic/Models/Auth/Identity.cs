using Clinic.Models.Catalogs;
using Clinic.Models.Core;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.Auth
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseEntity
    {
        /// <summary>
        /// Register datetime
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Enum for save current status of record
        /// </summary>
        public StatusCore Status { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        [StringLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Paternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        public string PaternalSurname { get; set; } = string.Empty;

        /// <summary>
        /// Maternal surname of user
        /// </summary>
        [StringLength(50)]
        [Required]
        public string MaternalSurname { get; set; } = string.Empty;

        /// <summary>
        /// Get Name complete of user
        /// </summary>
        public string NameComplete => $"{Name} {PaternalSurname} {MaternalSurname}";


        /// <summary>
        /// List of Claims
        /// </summary>
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();

        /// <summary>
        /// List of logins
        /// </summary>
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; } = new List<ApplicationUserLogin>();

        /// <summary>
        /// List of tokens
        /// </summary>
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; } = new List<ApplicationUserToken>();

        /// <summary>
        /// List of User roles
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }

    /// <summary>
    /// Class for define roles
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>
        /// Status of role
        /// </summary>
        public StatusCore Status { get; set; }

        /// <summary>
        /// Description for role
        /// </summary>
        [StringLength(180)]
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// List of user roles
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        /// <summary>
        /// List of role claims
        /// </summary>
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = new List<ApplicationRoleClaim>();
    }

    /// <summary>
    /// Class for define user roles (intermediate table)
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        /// <summary>
        /// Navigation property for User
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Navigation property for Role
        /// </summary>
        public virtual ApplicationRole? Role { get; set; }
    }

    /// <summary>
    /// Class for define user claims
    /// </summary>
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        /// <summary>
        /// Navigation property for user
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
    }

    /// <summary>
    /// Class for define user logins
    /// </summary>
    public class ApplicationUserLogin : IdentityUserLogin<Guid>
    {
        /// <summary>
        /// Navigation property for user
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
    }

    /// <summary>
    /// Class for define role claims
    /// </summary>
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        /// <summary>
        /// Navigation property for role
        /// </summary>
        public virtual ApplicationRole? Role { get; set; }
    }

    /// <summary>
    /// Class for define user tokens
    /// </summary>
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        /// <summary>
        /// Navigation property for user
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
    }
}
