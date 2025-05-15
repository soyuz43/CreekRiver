using Microsoft.EntityFrameworkCore;
using CreekRiver.Models;

public class CreekRiverDbContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Campsite> Campsites { get; set; }
    public DbSet<CampsiteType> CampsiteTypes { get; set; }

    public CreekRiverDbContext(DbContextOptions<CreekRiverDbContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampsiteType>().HasData(new CampsiteType[]
        {
            new() { Id = 1, CampsiteTypeName = "Tent", FeePerNight = 15.99M, MaxReservationDays = 7 },
            new() { Id = 2, CampsiteTypeName = "RV", FeePerNight = 26.50M, MaxReservationDays = 14 },
            new() { Id = 3, CampsiteTypeName = "Primitive", FeePerNight = 10.00M, MaxReservationDays = 3 },
            new() { Id = 4, CampsiteTypeName = "Hammock", FeePerNight = 12M, MaxReservationDays = 7 }
        });

        modelBuilder.Entity<Campsite>().HasData(new Campsite[]
        {
            new() { Id = 1, CampsiteTypeId = 1, Nickname = "Barred Owl", ImageUrl = "https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg" },
            new() { Id = 2, CampsiteTypeId = 2, Nickname = "Mossy Ridge", ImageUrl = "https://example.com/rvspot.jpg" },
            new() { Id = 3, CampsiteTypeId = 1, Nickname = "Otter Cove", ImageUrl = "https://example.com/otter.jpg" },
            new() { Id = 4, CampsiteTypeId = 3, Nickname = "Wren Hollow", ImageUrl = "https://example.com/primitive.jpg" },
            new() { Id = 5, CampsiteTypeId = 4, Nickname = "Hawk Perch", ImageUrl = "https://example.com/hammock.jpg" },
        });

        modelBuilder.Entity<UserProfile>().HasData(new UserProfile[]
        {
            new() { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
        });

        modelBuilder.Entity<Reservation>().HasData(new Reservation[]
        {
            new() { Id = 1, CampsiteId = 1, UserProfileId = 1, CheckinDate = new DateTime(2025, 6, 1), CheckoutDate = new DateTime(2025, 6, 4) }
        });
    }
}
