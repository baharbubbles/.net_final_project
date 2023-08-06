
// payment controller has 2 methods
// 1. StartPaymentProcess
// 1.1 This will return an reference number and adds this reference number to memory cache
// 2. CompletePaymentProcess
// 2.1 This will complete the payment process and returns the result

using ApartmentManagement.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ApartmentManagement.PaymentService;

[ApiController]
[Route("apartmng/api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IMemoryCache memoryCache;

    public PaymentController(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    [HttpPost("StartPaymentProcess")]
    public ApiResponse<string> StartPaymentProcess(int identityId)
    {
        var referenceNumber = ReferenceNumberGenerator.Get();
        memoryCache.Set(referenceNumber, identityId, TimeSpan.FromMinutes(5));
        var response = new ApiResponse<string>(referenceNumber);
        response.Response = referenceNumber;
        response.Success = true;
        return response;
    }

    [HttpPost("CompletePaymentProcess")]
    public ApiResponse CompletePaymentProcess(string referenceNumber)
    {
        var response = new ApiResponse();
        if (memoryCache.TryGetValue(referenceNumber, out int identityId))
        {
            memoryCache.Remove(referenceNumber);
            response.Success = true;
            response.Message = $"Payment completed for {identityId}";
        }
        else
        {
            response.Success = false;
            response.Message = "Reference number is not valid";
        }
        return response;
    }
}
