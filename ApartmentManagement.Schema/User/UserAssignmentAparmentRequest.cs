namespace ApartmentManagement.Schema;
public class UserAssignmentApartmentRequest
{
    public int UserId { get; set; }
    public int[] ApartmentIds { get; set; }

}