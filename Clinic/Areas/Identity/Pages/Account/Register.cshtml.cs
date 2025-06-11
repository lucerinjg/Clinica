using Clinic.Data;
using Clinic.Models.Auth;
using Clinic.Models.Catalogs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<RegisterModel> logger) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<RegisterModel> _logger = logger;


        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [Display(Name = "Nombre (s)")]
            public string Name { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Apellido paterno")]
            public string PaternalSurname { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Apellido materno")]
            public string MaternalSurname { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
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
        }
        public string? ReturnUrl { get; set; }
        //public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();


        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            ReturnUrl = returnUrl;

            HttpContext.Session.Clear();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await _signInManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var normalized = Input.Email.Normalize();
                var current = await _context.Users
                    .Where(x => x.NormalizedEmail == normalized)
                    .Select(x => new { x.Id, x.Status }).FirstOrDefaultAsync();
                if (current != null)
                {
                    ModelState.AddModelError(string.Empty, "Error. El correo electrónico ya fue registrado.");
                    return Page();
                }
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    PaternalSurname = Input.PaternalSurname,
                    MaternalSurname = Input.MaternalSurname,
                    CreationDate = DateTime.UtcNow,
                    Status = StatusCore.Active,
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _context.Patients.AddAsync(new Models.Patient
                    {
                        ApplicationUserId = user.Id,
                    });
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(user, "Patient");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(Url.Content("~/")); // Redirect to Home
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ModelState.AddModelError(string.Empty, "Error. Favor de verificar sus credenciales.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
