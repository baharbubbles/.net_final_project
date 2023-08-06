using ApartmentManagement.Base;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Repository;

namespace ApartmentManagement.Data.Uow;

public interface IUnitOfWork
{
    void Complete();
    void CompleteWithTransaction();

    IGenericRepository<Entity> DynamicRepository<Entity>() where Entity : BaseModel;
    IGenericRepository<Apartment> ApartmentRepository { get; }
    IGenericRepository<Message> MessageRepository { get; }
    IGenericRepository<Transaction> TransactionRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    IGenericRepository<UserLog> UserLogRepository { get; }



}