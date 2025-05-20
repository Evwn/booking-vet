namespace SimpleVetBooking.Data.Models;

public enum DayOfWeek
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public class WorkingHour
{
    public int Id { get; set; }
    public int VeterinarianId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    // Navigation property
    public Veterinarian? Veterinarian { get; set; }
} 