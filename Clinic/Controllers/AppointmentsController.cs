using Clinic.Data;
using Clinic.Models;
using Clinic.Models.Auth;
using Clinic.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Clinic.Controllers
{
    [Authorize(Policy = "Patients")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Guid GetUserId => new(_userManager.GetUserId(User) ?? "");


        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments
                .Where(x => x.PatientId == GetUserId)
                .Select(x => new AppointmentIndex
                {
                    AppointmentId = x.Id,
                    AppointmentStatus = x.Status,
                    TrackingId = x.TrackingId,
                    DoctorName = x.Doctor.ApplicationUser!.Name,
                    DoctorPaternalSurname = x.Doctor.ApplicationUser!.PaternalSurname,
                    DoctorMaternalSurname = x.Doctor.ApplicationUser!.MaternalSurname,
                    ReasonForVisit = x.ReasonForVisit,
                    SpecialtyName = x.Doctor.Specialty!.Name,
                    AppointmentDateTime = x.BookedSlot.StartTime,
                });
            return View(await applicationDbContext.ToListAsync());
        }

        private async Task GenerateSpecialtiesAsync()
        {
            var specialties = _context.Specialties
                .Where(d => d.Status == Models.Catalogs.StatusCore.Active)
                .Select(d => new { d.Id, d.Name });
            ViewBag.Specialties = new SelectList(await specialties.ToListAsync(), "Id", "Name");
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Where(x => x.Id == id)
                .Select(x => new AppointmentDetails
                {
                    AppointmentId = x.Id,
                    TrackingId = x.TrackingId,
                    AppointmentStatus = x.Status,
                    AppointmentDateTime = x.BookedSlot.StartTime,
                    ClientName = x.Patient.ApplicationUser!.Name,
                    ClientPaternalSurname = x.Patient.ApplicationUser!.PaternalSurname,
                    ClientMaternalSurname = x.Patient.ApplicationUser!.MaternalSurname,
                    AdministrativeNotes = x.AdministrativeNotes,
                    DoctorMaternalSurname = x.Doctor.ApplicationUser!.MaternalSurname,
                    DoctorName = x.Doctor.ApplicationUser!.Name,
                    DoctorPaternalSurname = x.Doctor.ApplicationUser!.PaternalSurname,
                    ReasonForVisit = x.ReasonForVisit,
                    SpecialtyName = x.Doctor.Specialty!.Name
                }).FirstOrDefaultAsync();
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            await GenerateSpecialtiesAsync();
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] AppointmentCreate appointment, [FromServices] IOptions<ClientOptions> options)
        {
            var opt = options.Value;

            if (ModelState.IsValid)
            {
                var speciality = await _context.Specialties.FindAsync(appointment.SpecialtyId);
                if (speciality == null || speciality.Status != Models.Catalogs.StatusCore.Active)
                {
                    await GenerateSpecialtiesAsync();
                    ModelState.AddModelError("SpecialtyId", "La especialidad seleccionada no es válida o está inactiva.");
                    return View();
                }
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == GetUserId);
                if (patient == null)
                {
                    await GenerateSpecialtiesAsync();
                    ModelState.AddModelError("", "Error inesperado. Favor de actualizar la página.");
                    return View();
                }
                var slot = await _context.DoctorSlots.FindAsync(appointment.SlotId);
                if (slot == null || slot.IsBooked || slot.StartTime < DateTime.Now)
                {
                    await GenerateSpecialtiesAsync();
                    ModelState.AddModelError("SlotId", "El horario seleccionado no está disponible o es inválido.");
                    return View(appointment);
                }
                var model = new Appointment
                {
                    TrackingId = $"{opt.Acronym}-{DateTime.Now:yyyyMMdd}-{speciality.Code}-{patient.Code:D4}",
                    PatientId = GetUserId,
                    DoctorId = appointment.DoctorId,
                    SlotId = appointment.SlotId,
                    Status = Models.Catalogs.AppointmentStatus.Scheduled,
                    ReasonForVisit = appointment.ReasonForVisit,
                    CreationDate = DateTime.Now,
                };
                _context.Appointments.Add(model);
                slot.IsBooked = true; // Marcar el horario como reservado
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            await GenerateSpecialtiesAsync();

            return View(appointment);
        }

        [HttpGet]
        public async Task<JsonResult> GetDoctors(Guid specialtyId)
        {
            var doctors = await _context.Doctors
                                .Where(d => d.SpecialtyId == specialtyId && d.ApplicationUser.Status == Models.Catalogs.StatusCore.Active)
                                .Select(d => new
                                {
                                    id = d.ApplicationUserId,
                                    text = d.ApplicationUser!.PaternalSurname + " " + d.ApplicationUser!.MaternalSurname + ", " + d.ApplicationUser!.Name
                                })
                                .ToListAsync();
            return Json(doctors);
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableSlots(Guid doctorId)
        {
            // Solo mostrar horarios futuros que no estén reservados.
            var slots = await _context.DoctorSlots
                            .Where(s => s.DoctorId == doctorId && !s.IsBooked && s.StartTime > DateTime.Now)
                            .OrderBy(s => s.StartTime)
                            .Select(s => new
                            {
                                id = s.Id,
                                text = s.StartTime.ToString("dddd, dd MMMM yyyy 'a las' HH:mm 'hrs.'") // Formato amigable
                            })
                            .ToListAsync();
            return Json(slots);
        }
    }
}
