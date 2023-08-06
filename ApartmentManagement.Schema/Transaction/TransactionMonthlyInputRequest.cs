using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Schema;

/// <summary>
/// Ay bazlı her bir daireye ait girişlerin daire bazlı toplu olarak girilmesi için kullanılır.
/// </summary>
public class TransactionMonthlyInputRequest
{
    public DateTime BillDate { get; set; }
    public List<TransactionMonthlyInputItem> Items { get; set; }
}

public class TransactionMonthlyInputItem
{
    public int ApartmentId { get; set; }
    public decimal Amount { get; set; }
    public Enum_BillType BillType { get; set; }
}

/// <summary>
/// Ay bazlı her bir daireye ait girişlerin toplu olarak girilmesi için kullanılır.
/// </summary>
public class TransactionMonthlyInputAllRequest
{
    public DateTime BillDate { get; set; }
    public decimal Amount { get; set; }
    public Enum_BillType BillType { get; set; }
}