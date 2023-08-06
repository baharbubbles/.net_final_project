using Microsoft.EntityFrameworkCore;
using ApartmentManagement.Data.Domain;

namespace ApartmentManagement.Data;

public class ApartmentManagementDbContext : DbContext
{
    public ApartmentManagementDbContext(DbContextOptions<ApartmentManagementDbContext> options) : base(options)
    {

    }


    // dbset
    public DbSet<Apartment>Apartments { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserLog> UserLog { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApartmentConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserLogConfiguration());

        base.OnModelCreating(modelBuilder);
    }



}