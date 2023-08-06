using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ApartmentManagement.Base;
using ApartmentManagement.Business;
using ApartmentManagement.Business.Token;
using ApartmentManagement.Data;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;
using ApartmentManagement.Service;
using System.Text;
using ApartmentManagement.Service.Clients;
using System.Reflection;

namespace ApartmentManagement.Service;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public static JwtConfig JwtConfig { get; private set; }


    public void ConfigureServices(IServiceCollection services)
    {

        services.AddControllers();

        JwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfig>();
        services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

        //dbcontext 
        var dbConfig = Configuration.GetConnectionString("MsSqlConnection");
        services.AddDbContext<ApartmentManagementDbContext>(opts => opts.UseSqlServer(dbConfig));
        
       
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        services.AddSingleton(config.CreateMapper());

        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserLogService, UserLogService>();

        // Ödeme servisi PaymentServiceClient aracılığıla iletişim kuruyor.
        services.AddHttpClient<PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri(Configuration.GetSection("PaymentServiceBaseUrl").Value);
});


        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JwtConfig.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConfig.Secret)),
                ValidAudience = JwtConfig.Audience,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Apartment Management Api", Version = "v1.0" });


            var xmlFile = $"NetCore.SwaggerDoc.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Apartment Management Api",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, new string[] { }}
            });
        });


    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apartment v1"));
        }

        app.UseMiddleware<HeartBeatMiddleware>();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        Action<RequestProfilerModel> requestResponseHandler = requestProfilerModel =>
        {
            Log.Information("-------------Request-Begin------------");
            Log.Information(requestProfilerModel.Request);
            Log.Information(Environment.NewLine);
            Log.Information(requestProfilerModel.Response);
            Log.Information("-------------Request-End------------");
        };
        app.UseMiddleware<RequestLoggingMiddleware>(requestResponseHandler);



        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // migrate Db
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApartmentManagementDbContext>();
            dbContext.Database.Migrate();

            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = userService.IsUserExist(Configuration.GetSection("AdminUser").GetSection("Email").Value);
            if (user.Success == false)
            {
                userService.NewAdminUser(new UserRequest
                {
                    Name = Configuration.GetSection("AdminUser").GetSection("Name").Value,
                    Email = Configuration.GetSection("AdminUser").GetSection("Email").Value,
                    Password = Configuration.GetSection("AdminUser").GetSection("Password").Value
                });
            }
        }
    }
}