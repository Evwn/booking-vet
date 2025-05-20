using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleVetBooking.Data.Models;

namespace SimpleVetBooking.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        
        // Look for existing data
        if (context.Services.Any())
        {
            return;   // DB has been seeded
        }
        
        // Add services
        var services = new[]
        {
            new Service { Name = "Regular Check-up", Description = "Routine health examination for your pet", Price = 50, DurationMinutes = 30 },
            new Service { Name = "Vaccination", Description = "Essential vaccines for your pet's health", Price = 60, DurationMinutes = 20 },
            new Service { Name = "Emergency Care", Description = "Urgent medical attention for your pet", Price = 150, DurationMinutes = 60 },
            new Service { Name = "Surgery", Description = "Various surgical procedures for pets", Price = 350, DurationMinutes = 120 },
            new Service { Name = "Dental Care", Description = "Dental cleaning and oral health examination", Price = 80, DurationMinutes = 45 },
            new Service { Name = "Grooming", Description = "Professional grooming services for your pet", Price = 45, DurationMinutes = 60 }
        };
        
        context.Services.AddRange(services);
        await context.SaveChangesAsync();
        
        // Add veterinarians (with user accounts)
        var vetUsers = new[]
        {
            new AppUser { UserName = "sarah.johnson@vetclinic.com", Email = "sarah.johnson@vetclinic.com", EmailConfirmed = true, FirstName = "Sarah", LastName = "Johnson", IsVeterinarian = true },
            new AppUser { UserName = "michael.smith@vetclinic.com", Email = "michael.smith@vetclinic.com", EmailConfirmed = true, FirstName = "Michael", LastName = "Smith", IsVeterinarian = true },
            new AppUser { UserName = "emily.davis@vetclinic.com", Email = "emily.davis@vetclinic.com", EmailConfirmed = true, FirstName = "Emily", LastName = "Davis", IsVeterinarian = true }
        };
        
        foreach (var user in vetUsers)
        {
            await userManager.CreateAsync(user, " ");
        }
        
        // Add veterinarian profiles
        var veterinarians = new[]
        {
            new Veterinarian 
            { 
                UserId = vetUsers[0].Id, 
                Specialization = "Small Animals", 
                Biography = "Dr. Sarah Johnson has over 10 years of experience working with small animals. She specializes in preventive care and internal medicine.",
                PhotoUrl = "/images/vets/sarah-johnson.jpg",
                IsAvailable = true
            },
            new Veterinarian 
            { 
                UserId = vetUsers[1].Id, 
                Specialization = "Surgery", 
                Biography = "Dr. Michael Smith is our surgery specialist with expertise in both routine and complex surgical procedures for all types of pets.",
                PhotoUrl = "/images/vets/michael-smith.jpg",
                IsAvailable = true
            },
            new Veterinarian 
            { 
                UserId = vetUsers[2].Id, 
                Specialization = "Exotic Pets", 
                Biography = "Dr. Emily Davis specializes in exotic pet care, including birds, reptiles, and small mammals. She has a special interest in avian medicine.",
                PhotoUrl = "/images/vets/emily-davis.jpg",
                IsAvailable = true
            }
        };
        
        context.Veterinarians.AddRange(veterinarians);
        await context.SaveChangesAsync();
        
        // Add working hours for vets
        var workingHours = new List<WorkingHour>();
        
        foreach (var vet in veterinarians)
        {
            // Monday to Friday, 9 AM to 5 PM
            for (int i = 1; i <= 5; i++)
            {
                workingHours.Add(new WorkingHour
                {
                    VeterinarianId = vet.Id,
                    DayOfWeek = (Models.DayOfWeek)i,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
        }
        
        context.WorkingHours.AddRange(workingHours);
        await context.SaveChangesAsync();
        
        // Connect veterinarians with services
        var vetServices = new List<VeterinarianService>();
        
        // Sarah can do all services except surgery
        foreach (var service in services.Where(s => s.Name != "Surgery"))
        {
            vetServices.Add(new VeterinarianService
            {
                VeterinarianId = veterinarians[0].Id,
                ServiceId = service.Id
            });
        }
        
        // Michael can do all services
        foreach (var service in services)
        {
            vetServices.Add(new VeterinarianService
            {
                VeterinarianId = veterinarians[1].Id,
                ServiceId = service.Id
            });
        }
        
        // Emily can do all services except surgery
        foreach (var service in services.Where(s => s.Name != "Surgery"))
        {
            vetServices.Add(new VeterinarianService
            {
                VeterinarianId = veterinarians[2].Id,
                ServiceId = service.Id
            });
        }
        
        context.VeterinarianServices.AddRange(vetServices);
        await context.SaveChangesAsync();
        
        // Add a sample client user
        var clientUser = new AppUser 
        { 
            UserName = "john.doe@example.com", 
            Email = "john.doe@example.com", 
            EmailConfirmed = true, 
            FirstName = "John", 
            LastName = "Doe", 
            Address = "123 Main St, Anytown, USA",
            IsVeterinarian = false 
        };
        
        await userManager.CreateAsync(clientUser, "P@ssword1");
        
        // Add pets for the client
        var pets = new[]
        {
            new Pet
            {
                OwnerId = clientUser.Id,
                Name = "Max",
                Species = "Dog",
                Breed = "Golden Retriever",
                DateOfBirth = DateTime.UtcNow.AddYears(-3),
                MedicalHistory = "Vaccinated, neutered"
            },
            new Pet
            {
                OwnerId = clientUser.Id,
                Name = "Luna",
                Species = "Cat",
                Breed = "Siamese",
                DateOfBirth = DateTime.UtcNow.AddYears(-2),
                MedicalHistory = "Vaccinated, spayed"
            }
        };
        
        context.Pets.AddRange(pets);
        await context.SaveChangesAsync();
        
        // Add a few sample appointments
        var appointments = new[]
        {
            new Appointment
            {
                ClientId = clientUser.Id,
                VeterinarianId = veterinarians[0].Id,
                ServiceId = services.First(s => s.Name == "Regular Check-up").Id,
                PetId = pets[0].Id,
                StartTime = DateTime.UtcNow.AddDays(3).Date.AddHours(10),
                EndTime = DateTime.UtcNow.AddDays(3).Date.AddHours(10).AddMinutes(30),
                Status = AppointmentStatus.Confirmed,
                Notes = "Annual check-up"
            },
            new Appointment
            {
                ClientId = clientUser.Id,
                VeterinarianId = veterinarians[2].Id,
                ServiceId = services.First(s => s.Name == "Vaccination").Id,
                PetId = pets[1].Id,
                StartTime = DateTime.UtcNow.AddDays(5).Date.AddHours(14),
                EndTime = DateTime.UtcNow.AddDays(5).Date.AddHours(14).AddMinutes(20),
                Status = AppointmentStatus.Pending,
                Notes = "Booster vaccination"
            }
        };
        
        context.Appointments.AddRange(appointments);
        await context.SaveChangesAsync();
    }
} 