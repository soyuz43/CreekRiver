namespace CreekRiver.DTOs;

public class CampsiteTypeDTO
{
    public int Id { get; set; }
    public string? CampsiteTypeName { get; set; }
    public decimal FeePerNight { get; set; }
    public int MaxReservationDays { get; set; }
}
