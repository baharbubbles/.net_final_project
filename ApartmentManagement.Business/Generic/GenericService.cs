using AutoMapper;
using Serilog;
using ApartmentManagement.Base;
using ApartmentManagement.Data.Uow;

namespace ApartmentManagement.Business.Generic;

public class GenericService<TEntity, TRequest, TResponse> : IGenericService<TEntity, TRequest, TResponse> where TEntity : BaseModel
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public GenericService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public virtual ApiResponse Delete(int Id)
    {
        try
        {
            var entity = unitOfWork.DynamicRepository<TEntity>().GetById(Id);
            if (entity == null)
            {
                return new ApiResponse("Record not found!");
            }

            unitOfWork.DynamicRepository<TEntity>().DeleteById(Id);
            unitOfWork.Complete();
            return new ApiResponse();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GenericService.Delete");
            return new ApiResponse(ex.Message);
        }
    }

    public virtual ApiResponse<List<TResponse>> GetAll(params string[] includes)
    {
        try
        {
            var entity = unitOfWork.DynamicRepository<TEntity>().GetAllWithInclude(includes);
            var mapped = mapper.Map<List<TEntity>, List<TResponse>>(entity);
            return new ApiResponse<List<TResponse>>(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GenericService.GetAll");
            return new ApiResponse<List<TResponse>>(ex.Message);
        }
    }

    public virtual ApiResponse<TResponse> GetById(int id, params string[] includes)
    {
        try
        {
            var entity = unitOfWork.DynamicRepository<TEntity>().GetByIdWithInclude(id, includes);
            var mapped = mapper.Map<TEntity, TResponse>(entity);
            return new ApiResponse<TResponse>(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GenericService.GetById");
            return new ApiResponse<TResponse>(ex.Message);
        }
    }

    public virtual ApiResponse Insert(TRequest request)
    {
        try
        {
            var entity = mapper.Map<TRequest, TEntity>(request);
            entity.InsertDate = DateTime.Now;
            entity.InsertUser = "something@admin.com";

            unitOfWork.DynamicRepository<TEntity>().Insert(entity);
            unitOfWork.DynamicRepository<TEntity>().Save();

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GenericService.Insert");
            return new ApiResponse(ex.Message);
        }
    }

    public virtual ApiResponse Update(int Id, TRequest request)
    {
        try
        {
            var exist = unitOfWork.DynamicRepository<TEntity>().GetById(Id);
            if (exist == null)
            {
                return new ApiResponse("Record not found!");
            }

            var entity = mapper.Map<TRequest, TEntity>(request);
            unitOfWork.DynamicRepository<TEntity>().Update(entity);
            unitOfWork.DynamicRepository<TEntity>().Save();

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GenericService.Update");
            return new ApiResponse(ex.Message);
        }
    }
}