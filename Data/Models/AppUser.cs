using Microsoft.AspNetCore.Identity;

namespace SimpleVetBooking.Data.Models;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsVeterinarian { get; set; }
    
    // Navigation properties
    public Veterinarian? Veterinarian { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
    public List<Pet> Pets { get; set; } = new();
} 