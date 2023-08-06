using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApartmentManagement.Base;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;
using ApartmentManagement.Business;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagement.Business.Token;

public class TokenService : ITokenService
{

    private readonly IUnitOfWork unitOfWork;
    private readonly IUserLogService userLogService;
    private readonly JwtConfig jwtConfig;
    public TokenService(IUnitOfWork unitOfWork, 
    IUserLogService userLogService,
    IOptionsMonitor<JwtConfig> jwtConfig)
    {
        this.unitOfWork = unitOfWork;
        this.jwtConfig = jwtConfig.CurrentValue;
        this.userLogService = userLogService;
    }

    public ApiResponse<TokenResponse> Login(TokenRequest request)
    {
        if (request is null)
        {
            return new ApiResponse<TokenResponse>("Request was null");
        }
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        {
            return new ApiResponse<TokenResponse>("Request was null");
        }

        request.UserName = request.UserName.Trim().ToLower();
        request.Password = request.Password.Trim();


        var user = unitOfWork.UserRepository.Where(x => x.Email.Equals(request.UserName)).FirstOrDefault();
        if (user is null)
        {
            Log(request.UserName, LogType.InValidUserName);
            return new ApiResponse<TokenResponse>("Invalid user informations");
        }
        if (user.Password.ToLower() != HashStringGenerator.Encode(request.Password))
        {
            user.PasswordRetryCount++;
            user.LastActivity = DateTime.UtcNow;

            if (user.PasswordRetryCount > 3)
                user.Status = Base.Enums.Enum_UserStatus.Failed;

            unitOfWork.UserRepository.Update(user);
            unitOfWork.Complete();

            Log(request.UserName, LogType.WrongPassword);
            return new ApiResponse<TokenResponse>("Invalid user informations");
        }


        if (user.Status != Base.Enums.Enum_UserStatus.Active)
        {
            Log(request.UserName, LogType.InValidUserStatus);
            if (user.PasswordRetryCount > 3 && user.LastActivity.AddMinutes(10) > DateTime.UtcNow)
            {
                return new ApiResponse<TokenResponse>("Retry count exceded, please try again later");
            }
            return new ApiResponse<TokenResponse>("Invalid user status");
        }
        if (user.PasswordRetryCount > 3)
        {
            Log(request.UserName, LogType.PasswordRetryCountExceded);
            return new ApiResponse<TokenResponse>("Password retry count exceded");
        }

        user.LastActivity = DateTime.UtcNow;
        user.Status = Base.Enums.Enum_UserStatus.Active;


        unitOfWork.UserRepository.Update(user);
        unitOfWork.Complete();


        string token = Token(user);

        Log(request.UserName, LogType.LogedIn);

        TokenResponse response = new()
        {
            AccessToken = token,
            ExpireTime = DateTime.Now.AddMinutes(jwtConfig.AccessTokenExpiration),
            UserName = user.Email
        };

        return new ApiResponse<TokenResponse>(response);
    }
    private string Token(User user)
    {
        Claim[] claims = GetClaims(user);
        var secret = Encoding.ASCII.GetBytes(jwtConfig.Secret);

        var jwtToken = new JwtSecurityToken(
            jwtConfig.Issuer,
            jwtConfig.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(jwtConfig.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return accessToken;
    }


    private Claim[] GetClaims(User user)
    {
        var claims = new[]
        {
            new Claim("Email",user.Email),
            new Claim("UserId",user.Id.ToString()), 
            new Claim("Status",user.Status.ToString()),
            new Claim(ClaimTypes.Name,$"{user.Name}"),
            // endpoint rol bazlı yetkilendirme token üretilirken Role tipinde bir claims olarak eklenir.
            new Claim(ClaimTypes.Role, user.Type == Base.Enums.Enum_UserType.Admin ? "admin" : "user")
        };

        return claims;
    }

    private void Log(string username, string logType)
    {
        UserLogRequest request = new()
        {
            LogType = logType,
            UserName = username,
            TransactionDate = DateTime.UtcNow
        };
        userLogService.Insert(request);
    }

}