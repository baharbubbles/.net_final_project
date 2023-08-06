namespace ApartmentManagement.Schema;

public class UserLogResponse 
{
    public string UserName { get; set; }
    public DateTime TransactionDate { get; set; }
    public string LogType { get; set; }
}