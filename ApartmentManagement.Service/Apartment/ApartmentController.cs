using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Service;


[Authorize(Roles = "admin")]
[ApiController]
[Route("apartmng/api/[controller]")]
public class ApartmentController : ControllerBase
{
    private readonly IApartmentService service;
    public ApartmentController(IApartmentService service)
    {
        this.service = service;
    }


    /// <summary>
    /// Tüm daire listesini kullanıcısıyla birlikte döner
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse<List<ApartmentResponse>> GetAll()
    {        
        var response = service.GetAll("User");
        return response;
    }

    /// <summary>
    /// Id'ye göre daire döner
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ApiResponse<ApartmentResponse> Get(int id)
    {
        var response = service.GetById(id, "User");
        return response;
    }

    /// <summary>
    /// Yeni bir daire oluşturur.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public ApiResponse Post([FromBody] ApartmentRequest request)
    {
        var response = service.Insert(request);
        return response;
    }

    /// <summary>
    /// Daireyi günceller
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public ApiResponse Put(int id, [FromBody] ApartmentRequest request)
    {

        var response = service.Update(id, request);
        return response;
    }

    /// <summary>
    /// Daireyi siler
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ApiResponse Delete(int id)
    {
        var response = service.Delete(id);
        return response;
    }
}