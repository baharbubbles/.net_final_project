using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public interface IMessageService : IGenericService<Message,MessageRequest,MessageResponse>
{
    ApiResponse Send(int SenderId, MessageRequest request);

    ApiResponse Read(int id);

    ApiResponse<List<MessageResponse>> GetInbox(int receiverId);

    ApiResponse DeleteMessage(int senderId, int id);
}