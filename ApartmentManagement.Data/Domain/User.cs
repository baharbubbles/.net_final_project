using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Base;
using System.ComponentModel.DataAnnotations.Schema;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Data.Domain;


[Table("User", Schema = "dbo")]
public class User : IdBaseModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string TcNo { get; set; }
    public string PlateNumber { get; set; }
    public string ReferenceNumber { get; set; }
    public int PasswordRetryCount { get; set; }
    public DateTime LastActivity { get; set; }
    public Enum_UserStatus Status { get; set; }
    public Enum_UserType Type { get; set; }
    public virtual List<Apartment> Apartments { get; set; }

    public virtual List<Message> SentMessages { get; set; }
    public virtual List<Message> ReceivedMessages { get; set; }
}



public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Id).IsRequired(true).UseIdentityColumn();
        builder.Property(x => x.InsertUser).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.InsertDate).IsRequired(true);

        builder.Property(x => x.Name).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.Email).IsRequired(true).HasMaxLength(30);
        builder.Property(x => x.Password).IsRequired(true).HasMaxLength(100);
        builder.Property(x => x.TcNo).IsRequired(true).HasMaxLength(11);
        builder.Property(x => x.PhoneNumber).IsRequired(true).HasMaxLength(18);
        builder.Property(x => x.PlateNumber).IsRequired(false).HasMaxLength(15);
        builder.Property(x => x.ReferenceNumber).IsRequired(true).HasMaxLength(50);

        builder.Property(x => x.LastActivity).IsRequired(true);
        builder.Property(x => x.PasswordRetryCount).IsRequired(true);
        builder.Property(x => x.Status).IsRequired(true);

        builder.Property(x => x.Type).IsRequired(true).HasDefaultValue(Enum_UserType.User);
        builder.HasIndex(x => x.TcNo).IsUnique(true);


        builder.HasMany(x => x.Apartments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired(false);

        builder.HasMany(x => x.SentMessages)
            .WithOne(x => x.Sender)
            .HasForeignKey(x => x.SenderId)
            .IsRequired(true);
        
        builder.HasMany(x => x.ReceivedMessages)
            .WithOne(x => x.Receiver)
            .HasForeignKey(x => x.ReceiverId)
            .IsRequired(true);

        
    }
}