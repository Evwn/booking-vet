namespace SimpleVetBooking.Data.Models;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; } = 30;
    
    // Navigation properties
    public List<VeterinarianService> Veterinarians { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
} 