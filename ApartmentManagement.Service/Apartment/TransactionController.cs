using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Schema;
using ApartmentManagement.Base.Enums;
using ApartmentManagement.Service.Clients;
using AutoMapper;

namespace ApartmentManagement.Service;



[Authorize]
[ApiController]
[Route("apartmng/api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService service;
    private readonly PaymentServiceClient paymentServiceClient;
    private readonly IMapper mapper;
    public TransactionController(ITransactionService service, PaymentServiceClient paymentServiceClient, IMapper mapper)
    {
        this.service = service;
        this.paymentServiceClient = paymentServiceClient;
        this.mapper = mapper;
    }

    /// <summary>
    /// Tüm faturaları döner
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet]
    public ApiResponse<List<TransactionResponse>> GetAll()
    {
        var response = service.GetAll();
        return response;
    }

    /// <summary>
    /// Id'ye göre fatura döner
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet("{id}")]
    public ApiResponse<TransactionResponse> Get(int id)
    {
        var response = service.GetById(id);
        return response;
    }

    /// <summary>
    /// Kullanıcının faturalarını döner
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetMyTransactions")]
    public ApiResponse<List<TransactionResponse>> GetMyTransactions()
    {
        var response = service.GetByUserId(Convert.ToInt32(User.Claims.First(x=>x.Type == "UserId").Value));
        return response;
    }

    /// <summary>
    /// Fatura ödeme sürecini başlatır
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns>Geriye Referans numarası döner</returns>
    [Authorize]
    [HttpGet("StartPayment/{id}")]
    public async Task<ApiResponse<string>> StartPayment(int transactionId)
    {
        var transaction = service.GetById(transactionId);
        if (transaction.Success)
        {
            var response = await paymentServiceClient.StartPaymentProcess(transaction.Response.Apartment.UserId.Value);
            if (response.Success == false)
            {
                return new ApiResponse<string>("Payment process could not be started");
            }
            var serviceResponse = service.PaymentStart(transactionId, response.Response);
            if (serviceResponse.Success)
            {
                return response;
            }
            else
            {
                return new ApiResponse<string>("Payment process could not be started");
            }
        }
        else
        {
            return new ApiResponse<string>(transaction.Message);
        }
    }


    /// <summary>
    /// Ödeme sürecini tamamlar
    /// </summary>
    /// <param name="referenceNumber">StartPayment dan dönen referans numarası ile ödeme tamamlanabilir</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("CompletePayment/{referenceNumber}")]
    public async Task<ApiResponse> CompletePayment(string referenceNumber)
    {
        var transaction = service.GetByReference(referenceNumber).Response;
        if(transaction == null)
        {
            return new ApiResponse("Transaction not found");
        }
        var response = await paymentServiceClient.CompletePaymentProcess(referenceNumber);
        if (response.Success)
        {
            var serviceResponse = service.PaymentComplete(transaction.Id);
            return serviceResponse;
        }
        else
        {
            return new ApiResponse(response.Message);
        }
    }



    /// <summary>
    /// Referans numarasına göre fatura döner
    /// </summary>
    /// <param name="referenceNumber"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetByReference/{referenceNumber}")]
    public ApiResponse<TransactionResponse> GetByReference(string referenceNumber)
    {
        var response = service.GetByReference(referenceNumber);
        return response;
    }

    /// <summary>
    /// Fatura tarihine göre fatura döner
    /// </summary>
    /// <param name="billDate">Fatura Tarihi</param>
    /// <param name="billType">Fatura Tipi</param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet("GetByBillDate/{billDate}")]
    public ApiResponse<List<TransactionResponse>> GetByBillDate(DateTime billDate, [FromQuery]Enum_BillType? billType = null)
    {
        var response = service.GetByBillDate(billDate, billType);
        return response;
    }

    /// <summary>
    /// Bir daireye ait faturaları döner
    /// </summary>
    /// <param name="apartmentNumber"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpGet("GetByApartmentId/{apartmentNumber}")]
    public ApiResponse<List<TransactionResponse>> GetByApartmentId(short apartmentNumber)
    {
        var response = service.GetByApartmentId(apartmentNumber);
        return response;
    }

    /// <summary>
    /// Ay bazlı ve daire daire fatura oluşturur
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpPost("BulkMonthlyInsert")]
    public ApiResponse BulkMonthlyInsert(TransactionMonthlyInputRequest request)
    {
        var response = service.BulkInsert(request);
        return response;
    }


    /// <summary>
    /// Ay bazlı tüm daireler için fatura oluşturur
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpPost("BulkMonthlyInsertAll")]
    public ApiResponse BulkMonthlyInsertAll(TransactionMonthlyInputAllRequest request)
    {
        var response = service.BulkInsertAll(request);
        return response;
    }

    /// <summary>
    /// Yeni bir fatura oluşturur
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpPost]
    public ApiResponse Post(TransactionRequest request)
    {
        var response = service.Insert(request);
        return response;
    }

    /// <summary>
    /// Fatura günceller
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpPut("{id}")]
    public ApiResponse Put(int id, TransactionRequest request)
    {
        var response = service.Update(id, request);
        return response;
    }

    /// <summary>
    /// Fatura siler
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles ="admin")]
    [HttpDelete("{id}")]
    public ApiResponse Delete(int id)
    {
        var response = service.Delete(id);
        return response;
    }

    
}