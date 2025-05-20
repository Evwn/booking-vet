using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SimpleVetBooking.Data;
using SimpleVetBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleVetBooking.Controllers
{
    [Route("api/booking")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public BookingController(ILogger<BookingController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("proceed-to-step3")]
        public IActionResult ProceedToStep3(int veterinarianId, [FromQuery] List<int> selectedServices)
        {
            if (selectedServices == null || !selectedServices.Any())
            {
                _logger.LogInformation("No services selected, redirecting back to Step 2");
                return Redirect($"/appointments/book/{veterinarianId}");
            }

            // Format the services as hyphen-separated string
            string serviceIds = string.Join("-", selectedServices);
            
            // Redirect to step 3 with the selected services
            return Redirect($"/appointments/book/{veterinarianId}/services/{serviceIds}");
        }
        
        [HttpGet("submit")]
        public IActionResult SubmitBooking(int veterinarianId, string serviceIds, int petId, string date, string time, string? notes = null)
        {
            try
            {
                _logger.LogInformation($"Submitting booking with: VetID={veterinarianId}, Services={serviceIds}, PetID={petId}, Date={date}, Time={time}, Notes={notes ?? "None"}");
                
                // Verify required fields
                if (veterinarianId <= 0 || string.IsNullOrEmpty(serviceIds) || petId <= 0 || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
                {
                    return BadRequest("Missing required fields");
                }
                
                // Parse date and time
                if (!DateTime.TryParse(date, out DateTime bookingDate))
                {
                    return BadRequest("Invalid date format");
                }
                
                // Parse time (format: HH-MM)
                var timeParts = time.Split('-');
                if (timeParts.Length != 2 || !int.TryParse(timeParts[0], out int hour) || !int.TryParse(timeParts[1], out int minute))
                {
                    return BadRequest("Invalid time format");
                }
                
                var startTime = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, hour, minute, 0);
                
                // Get user ID from claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                // Parse service IDs
                var serviceIdList = serviceIds.Split('-').Select(int.Parse).ToList();
                if (!serviceIdList.Any())
                {
                    return BadRequest("No services selected");
                }
                
                // Get primary service
                var primaryServiceId = serviceIdList.First();
                var primaryService = _dbContext.Services.FirstOrDefault(s => s.Id == primaryServiceId);
                if (primaryService == null)
                {
                    return BadRequest("Invalid service");
                }
                
                // Create appointment
                var appointment = new Appointment
                {
                    ClientId = userId,
                    VeterinarianId = veterinarianId,
                    ServiceId = primaryServiceId,
                    PetId = petId,
                    StartTime = startTime,
                    EndTime = startTime.AddMinutes(primaryService.DurationMinutes),
                    Notes = notes ?? "",
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Add additional services to notes if more than one service
                if (serviceIdList.Count > 1)
                {
                    var additionalServiceIds = serviceIdList.Skip(1);
                    var additionalServices = _dbContext.Services
                        .Where(s => additionalServiceIds.Contains(s.Id))
                        .Select(s => s.Name)
                        .ToList();
                    
                    if (additionalServices.Any())
                    {
                        appointment.Notes += $"\n\nAdditional services requested: {string.Join(", ", additionalServices)}";
                    }
                }
                
                _dbContext.Appointments.Add(appointment);
                _dbContext.SaveChanges();
                
                return Redirect("/appointments?success=true");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting booking: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your booking");
            }
        }

        [HttpPost("select-pet")]
        public IActionResult SelectPet(int? veterinarianId, string? serviceIds, int? petId, string? date, string? time, string? notes = null)
        {
            try
            {
                _logger.LogInformation($"Pet selection form received: VetID={veterinarianId}, Services={serviceIds ?? "NULL"}, PetID={petId}, Date={date ?? "NULL"}, Time={time ?? "NULL"}");
                
                // Try to get values from the form if they're not coming in as parameters
                if (!veterinarianId.HasValue && int.TryParse(Request.Form["veterinarianId"], out int formVetId) && formVetId > 0)
                {
                    veterinarianId = formVetId;
                }
                
                if (string.IsNullOrEmpty(serviceIds) && Request.Form.ContainsKey("serviceIds"))
                {
                    serviceIds = Request.Form["serviceIds"];
                }
                
                if (!petId.HasValue && int.TryParse(Request.Form["petId"], out int formPetId) && formPetId > 0)
                {
                    petId = formPetId;
                }
                
                if (string.IsNullOrEmpty(date) && Request.Form.ContainsKey("date"))
                {
                    date = Request.Form["date"];
                }
                
                if (string.IsNullOrEmpty(time) && Request.Form.ContainsKey("time"))
                {
                    time = Request.Form["time"];
                }
                
                // Validate all required parameters
                if (!veterinarianId.HasValue || veterinarianId <= 0)
                {
                    return BadRequest("Veterinarian ID is required");
                }
                
                if (string.IsNullOrEmpty(serviceIds))
                {
                    return BadRequest("Service IDs are required");
                }
                
                if (!petId.HasValue || petId <= 0)
                {
                    return BadRequest("Pet ID is required");
                }
                
                if (string.IsNullOrEmpty(date))
                {
                    return BadRequest("Date is required");
                }
                
                if (string.IsNullOrEmpty(time))
                {
                    return BadRequest("Time is required");
                }
                
                // Verify the pet ID is valid
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                
                var pet = _dbContext.Pets.FirstOrDefault(p => p.Id == petId && p.OwnerId == userId);
                
                if (pet == null)
                {
                    return BadRequest("Invalid pet ID");
                }
                
                // Build the URL for the confirmation page
                var confirmUrl = $"/appointments/book/{veterinarianId}/services/{serviceIds}/confirm?date={date}&time={time}&petId={petId}";
                
                // Add notes if provided
                if (!string.IsNullOrEmpty(notes))
                {
                    confirmUrl += $"&notes={System.Web.HttpUtility.UrlEncode(notes)}";
                }
                
                _logger.LogInformation($"Redirecting to confirmation page: {confirmUrl}");
                
                // Redirect to the confirmation page
                return Redirect(confirmUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing pet selection: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your selection");
            }
        }

        [HttpGet("vet/appointments/status/{appointmentId}/{status}")]
        public IActionResult UpdateAppointmentStatus(int appointmentId, string status)
        {
            // Get the current user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Please log in to continue");
            }
            
            // Check if the user is a veterinarian
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null || !user.IsVeterinarian)
            {
                return Forbid("Only veterinarians can update appointment status");
            }
            
            // Get the veterinarian record
            var veterinarian = _dbContext.Veterinarians.FirstOrDefault(v => v.UserId == userId);
            if (veterinarian == null)
            {
                return BadRequest("Veterinarian record not found");
            }
            
            // Get the appointment
            var appointment = _dbContext.Appointments
                .FirstOrDefault(a => a.Id == appointmentId && a.VeterinarianId == veterinarian.Id);
            
            if (appointment == null)
            {
                return NotFound($"Appointment #{appointmentId} not found or not assigned to this veterinarian");
            }
            
            // Parse and update the status
            if (Enum.TryParse<AppointmentStatus>(status, true, out var newStatus))
            {
                // Update the appointment status
                appointment.Status = newStatus;
                _dbContext.SaveChanges();
                
                // Redirect back to the appointment details page
                return Redirect($"/vet/appointments/{appointmentId}");
            }
            else
            {
                return BadRequest("Invalid status value");
            }
        }
    }
} 