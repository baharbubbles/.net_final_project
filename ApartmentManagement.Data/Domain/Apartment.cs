using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Base;
using System.ComponentModel.DataAnnotations.Schema;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Data.Domain;

[Table("Apartment", Schema = "dbo")]
public class Apartment : IdBaseModel
{
    public short ApartmentNumber { get; set; }
    public string Block { get; set; }

    public Enum_ApartmentStatus Status { get; set; }
    public Enum_ApartmentType Type { get; set; }
    public short Floor { get; set; }
    public int? UserId { get; set; }

    public virtual User User { get; set; }

    public virtual List<Transaction> Transactions { get; set; }
}



public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.Property(x => x.Id).IsRequired(true).UseIdentityColumn();
        builder.Property(x => x.InsertUser).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.InsertDate).IsRequired(true);
        builder.Property(x => x.ApartmentNumber).IsRequired(true);

        builder.Property(x => x.InsertUser).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.InsertDate).IsRequired(true);

        builder.Property(x => x.Block).IsRequired(true).HasMaxLength(4);
        builder.Property(x => x.Status).IsRequired(true).HasDefaultValue(Enum_ApartmentStatus.Empty);
        builder.Property(x => x.Type).IsRequired(true).HasDefaultValue(Enum_ApartmentType.x3_1);

        builder.Property(x => x.Floor).IsRequired(true).HasDefaultValue((short)1);

        builder.HasMany(x=>x.Transactions)
            .WithOne(x=>x.Apartment)
            .HasForeignKey(x=>x.ApartmentId)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Apartments)
            .HasForeignKey(x => x.UserId)
            .IsRequired(false);

    }
}