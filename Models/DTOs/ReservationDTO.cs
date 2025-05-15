namespace CreekRiver.Models.DTOs;

public class ReservationDTO
{
    public int Id { get; set; }
    public int CampsiteId { get; set; }
    public int UserProfileId { get; set; }
    public DateTime CheckinDate { get; set; }
    public DateTime CheckoutDate { get; set; }
}
