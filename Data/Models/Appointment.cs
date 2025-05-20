namespace SimpleVetBooking.Data.Models;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}

public class Appointment
{
    public int Id { get; set; }
    public string? ClientId { get; set; }
    public int VeterinarianId { get; set; }
    public int ServiceId { get; set; }
    public int? PetId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AppUser? Client { get; set; }
    public Veterinarian? Veterinarian { get; set; }
    public Service? Service { get; set; }
    public Pet? Pet { get; set; }
} 