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
    public class LoginModel(SignInManager<ApplicationUser> signInManager, ApplicationDbContext context) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ApplicationDbContext _context = context;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "¿Recordarme?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            HttpContext.Session.Clear();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await _signInManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var normalized = Input.Email.Normalize();
                var current = await _context.Users
                    .Where(x => x.NormalizedEmail == normalized)
                    .Select(x => new { x.Id, x.Status }).FirstOrDefaultAsync();
                if (current == null)
                {
                    ModelState.AddModelError(string.Empty, "Error. Favor de verificar sus credenciales.");
                    return Page();
                }
                if (current.Status != StatusCore.Active)
                {
                    ModelState.AddModelError(string.Empty, "Error. La cuenta ha sido deshabilitada.");
                    return Page();
                }
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(Url.Content("~/")); // Redirect to Home
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error. Favor de verificar sus credenciales.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
