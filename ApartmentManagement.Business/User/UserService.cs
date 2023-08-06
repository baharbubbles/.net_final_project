using AutoMapper;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;
using ApartmentManagement.Base;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Business;

public class UserService : GenericService<User, UserRequest, UserResponse>, IUserService
{

    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public UserService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    
    public ApiResponse<string> InsertUser(UserRequest request)
    {
        var password = PasswordGenerator.Get();
        var userModel = mapper.Map<User>(request);
        userModel.Type = Enum_UserType.User;
        userModel.ReferenceNumber = ReferenceNumberGenerator.Get();
        userModel.Status = Enum_UserStatus.Active;
        userModel.LastActivity = DateTime.Now;
        userModel.PasswordRetryCount = 0;
        userModel.Password = HashStringGenerator.Encode(password);
        unitOfWork.UserRepository.Insert(userModel);
        unitOfWork.UserRepository.Save();
        var response = new ApiResponse<string>("User created successfully");
        response.Response = password;
        response.Success = true;
        return response;
    }

    public ApiResponse IsUserExist(string email) {
        var user = unitOfWork.UserRepository.GetAllAsQueryable().Where(x => x.Email == email).Any();
        if(user == false)
            return new ApiResponse("User not found");
        return new ApiResponse();
    }

    public void NewAdminUser(UserRequest request)
    {
        var userModel = mapper.Map<User>(request);
        userModel.Password = HashStringGenerator.Encode(request.Password);
        userModel.Type = Enum_UserType.Admin;
        userModel.PhoneNumber = "00000000000";
        userModel.ReferenceNumber = ReferenceNumberGenerator.Get();
        userModel.Status = Enum_UserStatus.Active;
        userModel.LastActivity = DateTime.Now;
        userModel.PasswordRetryCount = 0;
        userModel.InsertDate = DateTime.Now;
        userModel.InsertUser = "System";
        userModel.TcNo = "00000000000";
        unitOfWork.UserRepository.Insert(userModel);
        unitOfWork.UserRepository.Save();
    }

    public ApiResponse AssignToApartment(UserAssignmentApartmentRequest request)
    {
        var user = unitOfWork.UserRepository.GetById(request.UserId);
        foreach (int apartmentId in request.ApartmentIds)
        {
            var apartment = unitOfWork.ApartmentRepository.GetById(apartmentId);
            if(apartment == null)
                return new ApiResponse("Apartment not found");
            
            apartment.UserId = user.Id;
            unitOfWork.ApartmentRepository.Update(apartment);
        }


        return new ApiResponse();
    }
}