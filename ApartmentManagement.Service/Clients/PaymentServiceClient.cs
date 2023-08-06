using ApartmentManagement.Base;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Service.Clients
{
    public class PaymentServiceClient
    {
        private readonly HttpClient httpClient;
        public PaymentServiceClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Yeni bir ödeme süreci başlatır
        /// </summary>
        /// <param name="identityId">Kullanıcı kimlik numarası</param>
        /// <returns>Ödeme için referans numarası döner</returns>
        public async Task<ApiResponse<string>> StartPaymentProcess(int identityId)
        {
            var response = await httpClient.PostAsJsonAsync("apartmng/api/Payment/StartPaymentProcess", identityId);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            return result;
        }

        /// <summary>
        /// Ödeme sürecini tamamlar
        /// </summary>
        /// <param name="referenceNumber">Ödeme Referans Numarası</param>
        /// <returns></returns>
        public async Task<ApiResponse> CompletePaymentProcess(string referenceNumber)
        {
            var response = await httpClient.PostAsJsonAsync("apartmng/api/Payment/CompletePaymentProcess", referenceNumber);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return result;
        }
    }
}

