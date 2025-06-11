using Clinic.Data;
using Clinic.Models;
using Clinic.Models.Auth;
using Clinic.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    [Authorize(Policy = "Doctors")]
    public class DoctorSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorSlotsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Guid GetUserId => new(_userManager.GetUserId(User) ?? "");


        // GET: DoctorSlots
        public async Task<IActionResult> Index()
        {
            var data = await _context.DoctorSlots
                .Where(x => x.DoctorId == GetUserId)
                .Select(x => new SlotsIndex
                {
                    SlotId = x.Id,
                    DoctorId = x.DoctorId,
                    Name = x.Doctor.ApplicationUser!.Name,
                    PaternalSurname = x.Doctor.ApplicationUser.PaternalSurname,
                    MaternalSurname = x.Doctor.ApplicationUser.MaternalSurname,
                    IsBooked = x.IsBooked,
                    StartDate = x.StartTime,
                    EndDate = x.EndTime,
                    Status = x.Status
                })
                .OrderBy(x => x.StartDate).ToListAsync();

            return View(data);
        }

        // GET: DoctorSlots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DoctorSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] SlotsCreate model)
        {
            if (ModelState.IsValid)
            {
                // Check dates
                if (model.StartDate > model.EndDate)
                {
                    ModelState.AddModelError("", "La fecha de inicio no puede ser mayor que la fecha de finalización");
                    return View(model);
                }

                if (model.StartDate < DateTime.Now)
                {
                    ModelState.AddModelError("", "No se pueden agregar citas antes de la fecha y hora actual");
                    return View(model);
                }

                var userId = GetUserId;

                // Check for any existing slots in the range
                var existingSlots = await _context.DoctorSlots
                    .Where(slot => slot.DoctorId == userId &&
                                   slot.StartTime >= model.StartDate &&
                                   slot.EndTime <= model.EndDate)
                    .AnyAsync();

                if (existingSlots)
                {
                    ModelState.AddModelError("", "Ya existen horarios programados en este rango de fechas.");
                    return View(model);
                }

                var r = AppointmentScheduler.GenerateSlots(
                    doctorId: userId,
                    rangeStart: model.StartDate,
                    rangeEnd: model.EndDate,
                    slotDurationInMinutes: model.DurationInMinutes);

                try
                {

                    await _context.DoctorSlots.AddRangeAsync(r);

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                    ModelState.AddModelError("", "Error al generar las citas. Verifique la información.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Por favor, corrija los errores en el formulario.");
            return View(model);
        }

        // GET: DoctorSlots/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorSlot = await _context.DoctorSlots
                .Where(x => x.Id == id)
                .Select(x => new SlotsEdit
                {
                    SlotId = x.Id,
                    Status = x.Status,
                }).FirstOrDefaultAsync();
            if (doctorSlot == null)
            {
                return NotFound();
            }

            return View(doctorSlot);
        }

        // POST: DoctorSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind] SlotsEdit doctorSlot)
        {
            if (id != doctorSlot.SlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var model = await _context.DoctorSlots.FindAsync(id);

                    if (model == null)
                    {
                        return NotFound();
                    }

                    model.Status = doctorSlot.Status;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!DoctorSlotExists(doctorSlot.SlotId))
                    {
                        //ModelState.AddModelError("", "El horario seleccionado no existe o ha sido eliminado.");
                        return NotFound();
                    }
                    else
                    {
                        _ = e.Message;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(doctorSlot);
        }

        private bool DoctorSlotExists(Guid id)
        {
            return _context.DoctorSlots.Any(e => e.Id == id);
        }
    }
}
