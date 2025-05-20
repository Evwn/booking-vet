namespace SimpleVetBooking.Data.Models;

public class VeterinarianService
{
    public int VeterinarianId { get; set; }
    public int ServiceId { get; set; }
    
    // Navigation properties
    public Veterinarian? Veterinarian { get; set; }
    public Service? Service { get; set; }
} 