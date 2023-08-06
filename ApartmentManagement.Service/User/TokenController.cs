using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business.Token;
using ApartmentManagement.Schema;
using Microsoft.AspNetCore.Authorization;

namespace ApartmentManagement.Service;

[ApiController]
[Route("apartmng/api/[controller]")]
public class TokenController : ControllerBase
{

    private readonly ITokenService service;
    public TokenController(ITokenService service)
    {
        this.service = service;
    }


    /// <summary>
    /// Uygulama çalışıyor mu kontrolü için yapılmıştır.
    /// </summary>
    /// <returns></returns>
    [TypeFilter(typeof(LogResourceFilter))]
    [TypeFilter(typeof(LogActionFilter))]
    [TypeFilter(typeof(LogAuthorizationFilter))]
    [TypeFilter(typeof(LogResultFilter))]
    [TypeFilter(typeof(LogExceptionFilter))]
    [HttpGet("HeartBeat")]
    public ApiResponse Get()
    {
        return new ApiResponse("Hello");
    }


    /// <summary>
    /// Kullanıcılar (yönetici ve daire kullanıcısı) bu uç aracılığıyla giriş yapar
    /// </summary>
    /// <param name="request">Kullanıcı adı(email) ve Parola bilgisi</param>
    /// <returns></returns>    
    [HttpPost("Login")]
    public ApiResponse<TokenResponse> Post([FromBody] TokenRequest request)
    {
        var response = service.Login(request);
        return response;
    }
}