using Clinic.Models;
using Clinic.Models.Auth;
using Clinic.Models.Catalogs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Data
{
    public static class InitialData
    {
        public static void ExecuteSeeds(IServiceScope host)
        {
            using var scope = host;
            // Get All services registers in Startup
            var services = scope.ServiceProvider;
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (EnsureContext(context).Result)
                {
                    AddRoles(context).Wait();
                    AddInitialCatalogs(context).Wait();
                    AddSudoUser(context, userManager).Wait();
                }
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        public static async Task<bool> EnsureContext(ApplicationDbContext context)
        {
            try
            {
                await context.Database.MigrateAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task AddRoles(ApplicationDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                await context.Roles.AddRangeAsync(new List<ApplicationRole> {
                    new() {
                        Id = Shared.ROL_CHANGE_PASSWORD_ID,
                        Status = StatusCore.Active,
                        Name = "ChangePassword",
                        NormalizedName = "CHANGEPASSWORD",
                        Description = "Requiere cambio de contraseña"
                    },
                    new() {
                        Id = Shared.ROL_SUDO_ID,
                        Status = StatusCore.Active,
                        Name = "Sudo",
                        NormalizedName = "SUDO",
                        Description = "Administrador"
                    },
                    new() {
                        Id = Shared.ROL_PATIENT,
                        Status = StatusCore.Active,
                        Name = "Patient",
                        NormalizedName = "PATIENT",
                        Description = "Paciente"
                    },
                    new() {
                        Id = Shared.ROL_DOCTOR,
                        Status = StatusCore.Active,
                        Name = "Doctor",
                        NormalizedName = "DOCTOR",
                        Description = "Doctor"
                    },
                });
                await context.SaveChangesAsync();
            }
        }

        public static async Task AddSudoUser(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!await context.Users.AnyAsync())
            {
                var user1 = new ApplicationUser
                {
                    Id = Shared.BASE_GUID_1,
                    Name = "Sistema",
                    PaternalSurname = "",
                    MaternalSurname = "",
                    Email = "sudo@mail.com",
                    UserName = "sudo@mail.com",
                    PhoneNumber = "",
                    CreationDate = DateTime.Now,
                    UserRoles =
                    [
                        new ApplicationUserRole{ RoleId = Shared.ROL_SUDO_ID }
                    ]
                };

                await userManager.CreateAsync(user1, "123qweasd!Sudo");
            }
        }


        public static async Task AddDoctors(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!await context.Users.AnyAsync())
            {
                var user1 = new ApplicationUser
                {
                    Id = Shared.BASE_GUID_1,
                    Name = "Alejandro",
                    PaternalSurname = "Cervantes",
                    MaternalSurname = "Márquez",
                    Email = "alejandro@lux.com",
                    UserName = "alejandro@lux.com",
                    PhoneNumber = "",
                    CreationDate = DateTime.Now,
                    UserRoles =
                    [
                        new ApplicationUserRole{ RoleId = Shared.ROL_DOCTOR }
                    ]
                };

                await userManager.CreateAsync(user1, "123qweasd!Alejandro");
            }
        }

        public static async Task AddInitialCatalogs(ApplicationDbContext context)
        {
            var now = DateTime.Now;

            if (!await context.Specialties.AnyAsync())
            {
                await context.Specialties.AddRangeAsync(new List<Specialty> 
                { 
                    new() { CreationDate = now, Code = "CR", Name = "Cardiología", Description = "Área de Cardiología", Status = StatusCore.Active },
                    new() { CreationDate = now, Code = "CG", Name = "Cirugía General", Description = "Área de Cirugía General", Status = StatusCore.Active },
                    new() { CreationDate = now, Code = "OR", Name = "Otorrinolaringología", Description = "Área de Otorrinolaringología", Status = StatusCore.Active },
                    new() { CreationDate = now, Code = "OT", Name = "Ortopedia y Traumatología", Description = "Área de Ortopedia y Traumatología", Status = StatusCore.Active },
               });
                await context.SaveChangesAsync();
            }
        }
    }
}
