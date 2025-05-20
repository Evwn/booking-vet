namespace SimpleVetBooking.Data.Models;

public class Veterinarian
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    // Navigation properties
    public AppUser? User { get; set; }
    public List<VeterinarianService> Services { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<WorkingHour> WorkingHours { get; set; } = new();
} 