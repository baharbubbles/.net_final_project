using Serilog;
using ApartmentManagement.Base;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Repository;

namespace ApartmentManagement.Data.Uow;

public class UnitOfWork : IUnitOfWork
{


    private readonly ApartmentManagementDbContext dbContext;
    public UnitOfWork(ApartmentManagementDbContext dbContext)
    {
        this.dbContext = dbContext;

        ApartmentRepository = new GenericRepository<Apartment>(dbContext);
        MessageRepository = new GenericRepository<Message>(dbContext);
        TransactionRepository = new GenericRepository<Transaction>(dbContext);
        UserRepository = new GenericRepository<User>(dbContext);
        UserLogRepository = new GenericRepository<UserLog>(dbContext);
    }

    public void Complete()
    {
        dbContext.SaveChanges();
    }

    public void CompleteWithTransaction()
    {
        using (var dbTransaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                dbContext.SaveChanges();
                dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbTransaction.Rollback();
                Log.Error(ex, "CompleteWithTransaction");
            }
        }
    }

    public IGenericRepository<Entity> DynamicRepository<Entity>() where Entity : BaseModel
    {
        return new GenericRepository<Entity>(dbContext);    
    }

    public IGenericRepository<Apartment> ApartmentRepository { get; private set; }

    public IGenericRepository<Message> MessageRepository { get; private set; }

    public IGenericRepository<Transaction> TransactionRepository { get; private set; }

    public IGenericRepository<User> UserRepository { get; private set; }
    public IGenericRepository<UserLog> UserLogRepository { get; private set; }

}