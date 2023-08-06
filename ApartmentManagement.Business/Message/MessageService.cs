using AutoMapper;
using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public class MessageService : GenericService<Message, MessageRequest, MessageResponse>, IMessageService
{

    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public MessageService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }
    public ApiResponse Send(int SenderId, MessageRequest request)
    {
        var message = new Message();
        mapper.Map(request, message);
        message.SenderId = SenderId;
        message.Type = Base.Enums.Enum_MessageType.New;
        unitOfWork.MessageRepository.Insert(message);
        unitOfWork.MessageRepository.Save();
        return new ApiResponse();
    }

    public ApiResponse Read(int id) 
    {
        var message = unitOfWork.MessageRepository.GetById(id);
        if(message == null)
            return new ApiResponse("Message not found");
        message.Type = Base.Enums.Enum_MessageType.Read;
        unitOfWork.MessageRepository.Update(message);
        unitOfWork.MessageRepository.Save();
        return new ApiResponse();
    }

    public ApiResponse<List<MessageResponse>> GetInbox(int receiverId)
    {
        var messages = unitOfWork.MessageRepository.GetAllAsQueryable().Where(x => x.ReceiverId == receiverId).ToList();
        var response = new ApiResponse<List<MessageResponse>>("");
        response.Response = mapper.Map<List<MessageResponse>>(messages);
        response.Success = true;
        return response;
    }

    public ApiResponse DeleteMessage(int senderId, int messageId) 
    {
        var message = unitOfWork.MessageRepository.GetById(messageId);
        if(message == null)
            return new ApiResponse("Message not found");
        if(message.SenderId != senderId)
            return new ApiResponse("Message not found");
        unitOfWork.MessageRepository.Delete(message);
        unitOfWork.MessageRepository.Save();
        return new ApiResponse();
    }
}