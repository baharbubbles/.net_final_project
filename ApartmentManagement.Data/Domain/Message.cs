using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Base;
using System.ComponentModel.DataAnnotations.Schema;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Data.Domain;

[Table("Message", Schema = "dbo")]
public class Message : IdBaseModel
{
    //PK
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }


    public string Content { get; set; }
    public Enum_MessageType Type { get; set; }



    public virtual User Sender { get; set; }
    public virtual User Receiver { get; set; }
}



public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(x => x.Id).IsRequired(true).UseIdentityColumn();


        builder.Property(x => x.InsertUser).IsRequired(true).HasMaxLength(50);
        builder.Property(x => x.InsertDate).IsRequired(true);

        builder.Property(x => x.Content).IsRequired(true);
        builder.Property(x => x.Type).IsRequired(true).HasDefaultValue(Enum_MessageType.New);
        
        builder.HasOne(x => x.Sender)
            .WithMany(x => x.SentMessages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(true);
        
        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.ReceivedMessages)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(true);
    }

}