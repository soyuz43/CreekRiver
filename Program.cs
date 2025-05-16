using CreekRiver.Models;
using CreekRiver.DTOs;
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
// POST /api/campsites
app.MapPost("/api/campsites", (CreekRiverDbContext db, Campsite campsite) =>
{
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


app.Run();
