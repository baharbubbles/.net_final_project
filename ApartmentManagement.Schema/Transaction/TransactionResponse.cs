using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Schema;

public class TransactionResponse
{
    public int Id { get; set; }
    public int ApartmentId { get; set; } 
    public ApartmentResponse? Apartment { get; set; }
    public decimal Amount { get; set; }
    public Enum_TransactionStatus Status { get; set; }
    public Enum_BillType BillType { get; set; }
    public DateTime BillDate { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? ReferenceNumber { get; set; }
}