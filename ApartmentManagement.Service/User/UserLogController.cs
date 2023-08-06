using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Schema;
using System.Security.Claims;

namespace ApartmentManagement.Service;

[Authorize(Roles = "admin")]
[ApiController]
[Route("apartmng/api/[controller]")]
public class UserLogController : ControllerBase
{
    private readonly IUserLogService service;
    public UserLogController(IUserLogService service)
    {
        this.service = service;
    }


    /// <summary>
    /// Tokendaki Kullanıcı için oturum işlemlerini döner. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ApiResponse<List<UserLogResponse>> GetAll()
    {
        var username = User.Claims.Where(x => x.Type == "UserName")?.FirstOrDefault();
        var userid = (User.Identity as ClaimsIdentity).FindFirst("UserId")?.Value;
        var response = service.GetByUserSession(username?.Value);
        return response;
    }

    /// <summary>
    /// Oturum bilgisini idsine göre döner
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ApiResponse<UserLogResponse> Get(int id)
    {
        var response = service.GetById(id);
        return response;
    }

    /// <summary>
    /// Oturum bilgisini siler
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