using Clinic.Data;
using Clinic.Models;
using Clinic.Models.Auth;
using Clinic.Models.Catalogs;
using Clinic.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Doctors
                .Select(x => new DoctorIndex
                {
                    Id = x.ApplicationUser!.Id,
                    Name = x.ApplicationUser!.Name,
                    PaternalSurname = x.ApplicationUser.PaternalSurname,
                    MaternalSurname = x.ApplicationUser.MaternalSurname,
                    Email = x.ApplicationUser.Email!,
                    PhoneNumber = x.ApplicationUser.PhoneNumber!,
                    SpecialtyId = x.SpecialtyId,
                    SpecialtyName = x.Specialty!.Name,
                    CreationDate = x.ApplicationUser.CreationDate,
                    Status = x.ApplicationUser.Status
                }).OrderBy(x => x.PaternalSurname)
                .ThenBy(x => x.MaternalSurname)
                .ThenBy(x => x.Name).ToListAsync());
        }

        // GET: Doctos/Create
        public async Task<IActionResult> Create()
        {
            var specialties = await _context.Specialties.AsNoTracking().Select(x => new
            {
                x.Id,
                x.Name,
            }).ToListAsync();

            ViewBag.Specialties = new SelectList(specialties, "Id", "Name");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] DoctorCreate model, [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    Name = model.Name,
                    PaternalSurname = model.PaternalSurname,
                    MaternalSurname = model.MaternalSurname,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    UserName = model.Email, // Use email as username
                    CreationDate = DateTime.UtcNow,
                    Status = StatusCore.Active,
                };
                var result = await userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, "Doctor");
                    await userManager.AddToRoleAsync(applicationUser, "ChangePassword");

                    // Save doctor details
                    var doctor = new Doctor
                    {
                        ApplicationUserId = applicationUser.Id,
                        SpecialtyId = model.SpecialtyId
                    };
                    _context.Doctors.Add(doctor);
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction(nameof(Index));
            }
            var specialties = await _context.Specialties.AsNoTracking().Select(x => new
            {
                x.Id,
                x.Name,
            }).ToListAsync();

            ViewBag.Specialties = new SelectList(specialties, "Id", "Name");
            return View(model);
        }

        // GET: Doctos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Doctors
                .Where(x => x.ApplicationUserId == id)
                .Select(x => new DoctorEdit
                {
                    Id = x.ApplicationUserId,
                    Name = x.ApplicationUser!.Name,
                    PaternalSurname = x.ApplicationUser.PaternalSurname,
                    MaternalSurname = x.ApplicationUser.MaternalSurname,
                    PhoneNumber = x.ApplicationUser.PhoneNumber,
                    Email = x.ApplicationUser.Email!,
                    SpecialtyId = x.SpecialtyId,
                    Status = x.ApplicationUser.Status
                }).FirstOrDefaultAsync();
            if (applicationUser == null)
            {
                return NotFound();
            }
            var specialties = await _context.Specialties.AsNoTracking().Select(x => new
            {
                x.Id,
                x.Name,
            }).ToListAsync();

            ViewBag.Specialties = new SelectList(specialties, "Id", "Name");
            return View(applicationUser);
        }

        // POST: Doctos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind] DoctorEdit model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = await _context.Doctors
                        .Include(d => d.ApplicationUser)
                        .FirstOrDefaultAsync(d => d.ApplicationUserId == id);

                    if (doctor == null)
                    {
                        return NotFound();
                    }

                    doctor.ApplicationUser.Name = model.Name;
                    doctor.ApplicationUser.PaternalSurname = model.PaternalSurname;
                    doctor.ApplicationUser.MaternalSurname = model.MaternalSurname;
                    doctor.ApplicationUser.PhoneNumber = model.PhoneNumber;
                    doctor.ApplicationUser.Email = model.Email;
                    doctor.SpecialtyId = model.SpecialtyId;
                    doctor.ApplicationUser.Status = model.Status;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ApplicationUserExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _ = ex.Message;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var specialties = await _context.Specialties.AsNoTracking().Select(x => new
            {
                x.Id,
                x.Name,
            }).ToListAsync();

            ViewBag.Specialties = new SelectList(specialties, "Id", "Name");
            return View(model);
        }

        private bool ApplicationUserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
