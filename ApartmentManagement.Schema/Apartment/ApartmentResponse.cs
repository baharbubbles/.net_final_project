using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Schema;

public class ApartmentResponse
{
    public int Id { get; set; }
    public short ApartmentNumber { get; set; }
    public string Block { get; set; }
    public Enum_ApartmentStatus Status { get; set; }
    public Enum_ApartmentType Type { get; set; }
    public short Floor { get; set; }
    public int? UserId { get; set; }
    public UserResponse? User { get; set; }
}