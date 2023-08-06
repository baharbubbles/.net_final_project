using ApartmentManagement.Base;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business.Token;

public interface ITokenService
{
    ApiResponse<TokenResponse> Login(TokenRequest request);
}