using ApartmentManagement.Base;

namespace ApartmentManagement.Business.Generic;

public interface IGenericService<TEntity,TRequest,TResponse>
{
    ApiResponse<List<TResponse>> GetAll(params string[] includes);
    ApiResponse<TResponse> GetById(int id, params string[] includes);
    ApiResponse Insert(TRequest request);
    ApiResponse Update(int Id, TRequest request);
    ApiResponse Delete(int Id);
}