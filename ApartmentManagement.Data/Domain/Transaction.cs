using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Base;
using System.ComponentModel.DataAnnotations.Schema;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Data.Domain;

[Table("Transaction", Schema = "dbo")]
public class Transaction : IdBaseModel
{
    public int ApartmentId { get; set; }
    public virtual Apartment Apartment { get; set; }


    public decimal Amount { get; set; }  
    public Enum_TransactionStatus Status { get; set; }
    public Enum_BillType BillType { get; set; }

    public DateTime BillDate { get; set; }    
    public DateTime? TransactionDate { get; set; } 
    public string? ReferenceNumber { get; set; }
}


public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {

        builder.Property(x => x.Id).IsRequired(true).UseIdentityColumn();
        builder.Property(x => x.InsertUser).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.InsertDate).IsRequired(true);

        builder.Property(x => x.BillDate).IsRequired(true);
        builder.Property(x => x.Status).IsRequired(true).HasDefaultValue(Enum_TransactionStatus.NotPaid);
        builder.Property(x => x.BillType).IsRequired(true).HasDefaultValue(Enum_BillType.Dues);
        builder.Property(x => x.Amount).IsRequired(true).HasPrecision(15, 4).HasDefaultValue(0);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(50);
        builder.Property(x => x.TransactionDate);

        builder.HasOne(x => x.Apartment)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.ApartmentId)
            .IsRequired(true);

    }

}