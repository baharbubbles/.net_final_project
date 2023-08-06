using AutoMapper;
using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;
using System.Reflection.Metadata;

namespace ApartmentManagement.Business;

public class ApartmentService : GenericService<Apartment, ApartmentRequest, ApartmentResponse>, IApartmentService
{

    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public ApartmentService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }



    public override ApiResponse<List<ApartmentResponse>> GetAll(params string[] includes)
    {
        return base.GetAll(includes);
    }

    public override ApiResponse<ApartmentResponse> GetById(int id, params string[] includes)
    {
        return base.GetById(id, includes);
    }

}