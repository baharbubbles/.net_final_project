using AutoMapper;
using ApartmentManagement.Data.Domain;

namespace ApartmentManagement.Schema;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<MessageRequest, Message>();
        CreateMap<Message, MessageResponse>();

        CreateMap<ApartmentRequest, Apartment>();
        CreateMap<Apartment, ApartmentResponse>();

        CreateMap<TransactionRequest, Transaction>();
        CreateMap<Transaction, TransactionResponse>();

        CreateMap<TransactionResponse, TransactionRequest>();

        CreateMap<UserRequest, User>();
        CreateMap<User, UserResponse>();

        CreateMap<UserLogRequest, UserLog>();
        CreateMap<UserLog, UserLogResponse>();

        CreateMap<UserLogRequest, UserLogResponse>();
    }
}