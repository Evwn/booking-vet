using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleVetBooking.Data.Models;

namespace SimpleVetBooking.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<VeterinarianService> VeterinarianServices => Set<VeterinarianService>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure VeterinarianService as a join table
        builder.Entity<VeterinarianService>()
            .HasKey(vs => new { vs.VeterinarianId, vs.ServiceId });
        
        builder.Entity<VeterinarianService>()
            .HasOne(vs => vs.Veterinarian)
            .WithMany(v => v.Services)
            .HasForeignKey(vs => vs.VeterinarianId);
        
        builder.Entity<VeterinarianService>()
            .HasOne(vs => vs.Service)
            .WithMany(s => s.Veterinarians)
            .HasForeignKey(vs => vs.ServiceId);
            
        // Ensure the DayOfWeek enum in WorkingHour doesn't conflict with System.DayOfWeek
        builder.Entity<WorkingHour>()
            .Property(wh => wh.DayOfWeek)
            .HasConversion<int>();
    }
} 