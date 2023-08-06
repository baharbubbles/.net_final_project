using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Service;

[Authorize(Roles = "admin")]
[ApiController]
[Route("apartmng/api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService service;
    public UserController(IUserService service)
    {
        this.service = service;
    }


    /// <summary>
    /// Tüm kullanıcı listesini döner
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse<List<UserResponse>> GetAll()
    {
        var response = service.GetAll();
        return response;
    }

    /// <summary>
    /// Id'ye göre kullanıcı döner
    /// </summary>
    /// <param name="id">kullanıcı id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ApiResponse<UserResponse> Get(int id)
    {
        var response = service.GetById(id);
        return response;
    }


    /// <summary>
    /// Yeni bir daire kullanıcısı oluşturur.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public ApiResponse<string> Post([FromBody] UserRequest request)
    {
        var response = service.InsertUser(request);
        return response;
    }


    /// <summary>
    /// Kullanıcı daireye atar
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("AssignToApartment")]
    public ApiResponse Post([FromBody] UserAssignmentApartmentRequest request)
    {
        var response = service.AssignToApartment(request);
        return response;
    }


    /// <summary>
    /// Kullanıcı günceller
    /// </summary>
    /// <param name="id">Kullanıcı Id</param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public ApiResponse Put(int id, [FromBody] UserRequest request)
    {

        var response = service.Update(id,request);
        return response;
    }


    /// <summary>
    /// Kullanıcıyı Siler
    /// </summary>
    /// <param name="id">Kullanıcı Id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ApiResponse Delete(int id)
    {
        var response = service.Delete(id);
        return response;
    }
}