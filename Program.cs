using CreekRiver.Models;
using CreekRiver.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Enable Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable legacy timestamp behavior for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Register DbContext with connection string from secrets
builder.Services.AddNpgsql<CreekRiverDbContext>(
    builder.Configuration["CreekRiverDbConnectionString"]);

var app = builder.Build();

// Swagger UI only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// GET /api/campsites
app.MapGet("/api/campsites", (CreekRiverDbContext db) =>
{
    return db.Campsites
        .Select(c => new CampsiteDTO
        {
            Id = c.Id,
            Nickname = c.Nickname,
            ImageUrl = c.ImageUrl,
            CampsiteTypeId = c.CampsiteTypeId
        })
        .ToList();
});
 

// GET /api/campsites/{id}
app.MapGet("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    var campsite = db.Campsites
        .Include(c => c.CampsiteType)
        .Select(c => new CampsiteDTO
        {
            Id = c.Id,
            Nickname = c.Nickname,
            ImageUrl = c.ImageUrl,
            CampsiteTypeId = c.CampsiteTypeId,
            CampsiteType = new CampsiteTypeDTO
            {
                Id = c.CampsiteType!.Id,  // 
                CampsiteTypeName = c.CampsiteType.CampsiteTypeName,
                FeePerNight = c.CampsiteType.FeePerNight,
                MaxReservationDays = c.CampsiteType.MaxReservationDays
            }
        })
        .SingleOrDefault(c => c.Id == id);

    return campsite is null ? Results.NotFound() : Results.Ok(campsite);
});


// POST /api/campsites — Create a campsite, always visible by default
app.MapPost("/api/campsites", (CreekRiverDbContext db, Campsite campsite) =>
{
    bool campsiteExists = db.Campsites.Any(c => c.Nickname == campsite.Nickname);
    if (campsiteExists)
    {
        return Results.BadRequest();
    }

    campsite.IsVisible = true;
    db.Campsites.Add(campsite);
    db.SaveChanges();
    return Results.Created($"/api/campsites/{campsite.Id}", campsite);
});


// DELETE /api/campsites/{id}
app.MapDelete("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    Campsite? campsite = db.Campsites.SingleOrDefault(c => c.Id == id);
    if (campsite == null)
    {
        return Results.NotFound();
    }

    db.Campsites.Remove(campsite);
    db.SaveChanges();
    return Results.NoContent();
});





// ! Campsite Visibility
// PATCH /api/campsites/{id}/visible — Toggle visibility via a DTO
app.MapPatch("/api/campsites/{id}/visible", (CreekRiverDbContext db, int id, CampsiteVisibleDTO campsiteVisibleDTO) =>
{
    var campsiteToUpdate = db.Campsites.SingleOrDefault(c => c.Id == id);
    if (campsiteToUpdate is null)
    {
        return Results.NotFound();
    }

    campsiteToUpdate.IsVisible = campsiteVisibleDTO.IsVisible;
    db.SaveChanges();
    return Results.NoContent();
});


// GET /api/reservations
app.MapGet("/api/reservations", (CreekRiverDbContext db) =>
{
    return db.Reservations
        .Include(r => r.UserProfile)
        .Include(r => r.Campsite)
            .ThenInclude(c => c!.CampsiteType)
        .OrderBy(r => r.CheckinDate)
        .Select(r => new ReservationDTO
        {
            Id = r.Id,
            CampsiteId = r.CampsiteId,
            UserProfileId = r.UserProfileId,
            CheckinDate = r.CheckinDate,
            CheckoutDate = r.CheckoutDate,
            UserProfile = new UserProfileDTO
            {
                Id = r.UserProfile!.Id,
                FirstName = r.UserProfile.FirstName,
                LastName = r.UserProfile.LastName,
                Email = r.UserProfile.Email
            },
            Campsite = new CampsiteDTO
            {
                Id = r.Campsite!.Id,
                Nickname = r.Campsite.Nickname,
                ImageUrl = r.Campsite.ImageUrl,
                CampsiteTypeId = r.Campsite.CampsiteTypeId,
                CampsiteType = new CampsiteTypeDTO
                {
                    Id = r.Campsite.CampsiteType!.Id,
                    CampsiteTypeName = r.Campsite.CampsiteType.CampsiteTypeName,
                    MaxReservationDays = r.Campsite.CampsiteType.MaxReservationDays,
                    FeePerNight = r.Campsite.CampsiteType.FeePerNight
                }
            }
        })
        .ToList();
});

// POST /api/reservations
app.MapPost("/api/reservations", (CreekRiverDbContext db, Reservation newRes) =>
{
    try
    {
        db.Reservations.Add(newRes);
        db.SaveChanges();
        return Results.Created($"/api/reservations/{newRes.Id}", newRes);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
});


app.Run();
