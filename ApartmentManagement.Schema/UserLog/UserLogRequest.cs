namespace ApartmentManagement.Schema;

public class UserLogRequest 
{
    public string UserName { get; set; }
    public DateTime TransactionDate { get; set; }
    public string LogType { get; set; }
}