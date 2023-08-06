using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public interface IUserService : IGenericService<User,UserRequest,UserResponse>
{
    ApiResponse AssignToApartment(UserAssignmentApartmentRequest request);
    ApiResponse IsUserExist(string email);
    ApiResponse<string> InsertUser(UserRequest request);
    void NewAdminUser(UserRequest request);
}