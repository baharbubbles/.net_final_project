using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Service;


[ApiController]
[Route("apartmng/api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService service;
    public MessageController(IMessageService service)
    {
        this.service = service;
    }


    /// <summary>
    /// Tüm mesaj listesini döner
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet]
    public ApiResponse<List<MessageResponse>> GetAll()
    {
        var response = service.GetAll();
        return response;
    }

    /// <summary>
    /// Id'ye göre mesaj döner
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet("{id}")]
    public ApiResponse<MessageResponse> Get(int id)
    {
        var response = service.GetById(id);
        return response;
    }

    /// <summary>
    /// Kullanıcıya gönderilen mesajları döner
    /// </summary>
    /// <returns></returns>
    [HttpGet("Inbox")]
    public ApiResponse<List<MessageResponse>> GetInbox()
    {
        var response = service.GetInbox(Convert.ToInt32(User.Claims.First(x=>x.Type == "UserId").Value));
        return response;
    }


    /// <summary>
    /// Bir kullanıcıya mesaj gönderir
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("Send")]
    public ApiResponse Send([FromBody] MessageRequest request)
    {
        var response = service.Send(Convert.ToInt32(User.Claims.First(x=>x.Type == "UserId").Value), request);
        return response;
    }



    /// <summary>
    /// Mesajı siler
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ApiResponse Delete(int id)
    {
        // Eğer kullanıcı admin değil ise sadece kendi gönderdiği mesajları siler
        if(User.IsInRole("admin") == false)
        {
            var response = service.DeleteMessage(Convert.ToInt32(User.Claims.First(x=>x.Type == "UserId").Value), id);
            return response;
        }
        // admin ise hepsini siler 
        else {
            var response = service.Delete(id);
            return response;
        }
    }
}