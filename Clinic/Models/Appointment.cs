using Clinic.Models.Catalogs;
using Clinic.Models.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    /// <summary>
    /// Main class that links a Patient, a Doctor, and a Time Slot.
    /// This is the central entity of the appointment module.
    /// </summary>
    public class Appointment : BaseEntity
    {

        [Required]
        [StringLength(25)] // E.g., LUX-20240925-A1B1234 // LUX-YYYYMMDD-[Speciality, 2][PatientCode, 5]
        public string TrackingId { get; set; } = string.Empty;

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid SlotId { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [StringLength(1000)]
        public string ReasonForVisit { get; set; } = string.Empty;

        public string AdministrativeNotes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey("SlotId")]
        public virtual DoctorSlot BookedSlot { get; set; } = null!;
    }

    /// <summary>
    /// Clase de utilidad para manejar la lógica de programación de citas.
    /// </summary>
    public static class AppointmentScheduler
    {
        /// <summary>
        /// Genera una lista de horarios disponibles (slots) dentro de un rango de tiempo específico
        /// para un doctor, dividiendo el tiempo en intervalos de una duración determinada.
        /// </summary>
        /// <param name="doctorId">El ID del doctor para el cual se generan los horarios.</param>
        /// <param name="rangeStart">La fecha y hora de inicio del rango a dividir (ej. 9:00 AM).</param>
        /// <param name="rangeEnd">La fecha y hora de fin del rango a dividir (ej. 2:00 PM).</param>
        /// <param name="slotDurationInMinutes">La duración de cada cita en minutos. Para tu caso, sería 20.</param>
        /// <returns>Una lista de objetos 'DoctorSlot', cada uno representando una cita potencial.</returns>
        public static List<DoctorSlot> GenerateSlots(Guid doctorId, DateTime rangeStart, DateTime rangeEnd, int slotDurationInMinutes = 20)
        {
            var creationDate = DateTime.Now;
            var slots = new List<DoctorSlot>();

            // Se normalizan los segundos y milisegundos para evitar problemas de precisión.
            var currentSlotStart = new DateTime(rangeStart.Year, rangeStart.Month, rangeStart.Day, rangeStart.Hour, rangeStart.Minute, 0);

            // El bucle continúa mientras el inicio del slot actual sea antes que el final del rango.
            while (currentSlotStart < rangeEnd)
            {
                var currentSlotEnd = currentSlotStart.AddMinutes(slotDurationInMinutes);

                // Se asegura de no crear un slot que termine después del rango permitido.
                if (currentSlotEnd > rangeEnd)
                {
                    break;
                }

                var newSlot = new DoctorSlot
                {
                    CreationDate = creationDate,
                    DoctorId = doctorId,
                    StartTime = currentSlotStart,
                    EndTime = currentSlotEnd,
                    IsBooked = false // Por defecto, un nuevo horario no está reservado.
                };

                slots.Add(newSlot);

                // Preparamos el inicio del siguiente slot.
                currentSlotStart = currentSlotEnd;
            }

            return slots;
        }
    }
}
