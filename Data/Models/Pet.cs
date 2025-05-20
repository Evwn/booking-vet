namespace SimpleVetBooking.Data.Models;

public class Pet
{
    public int Id { get; set; }
    public string? OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? MedicalHistory { get; set; }
    
    // Navigation properties
    public AppUser? Owner { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
} 