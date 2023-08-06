using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public interface IUserLogService : IGenericService<UserLog,UserLogRequest,UserLogResponse>
{
    ApiResponse<List<UserLogResponse>> GetByUserSession(string username);
}