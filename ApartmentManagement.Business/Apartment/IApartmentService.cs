using ApartmentManagement.Base;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public interface IApartmentService : IGenericService<Apartment, ApartmentRequest, ApartmentResponse>
{
    
}