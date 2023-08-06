using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Schema;

public class TransactionRequest
{
    public int ApartmentId { get; set; }
    public decimal Amount { get; set; }
    public Enum_BillType BillType { get; set; }
    public DateTime BillDate { get; set; }
}