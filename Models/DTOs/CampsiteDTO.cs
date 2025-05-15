namespace CreekRiver.DTOs;

public class CampsiteDTO
{
    public int Id { get; set; }
    public string?  Nickname { get; set; }
    public string? ImageUrl { get; set; }
    public int CampsiteTypeId { get; set; }
    public CampsiteTypeDTO? CampsiteType { get; set; }
}
