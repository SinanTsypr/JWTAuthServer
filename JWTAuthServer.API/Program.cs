using FluentValidation;
using FluentValidation.AspNetCore;
using JWTAuthServer.Core.Configurations;
using JWTAuthServer.Core.Models;
using JWTAuthServer.Core.Repositories;
using JWTAuthServer.Core.Services;
using JWTAuthServer.Core.UnitOfWork;
using JWTAuthServer.Data;
using JWTAuthServer.Data.Repositories;
using JWTAuthServer.Service.Services;
using JWTAuthServer.SharedLibrary.Configurations;
using JWTAuthServer.SharedLibrary.Exceptions;
using JWTAuthServer.SharedLibrary.Extensions;
using JWTAuthServer.SharedLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DI Register
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();



            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("JWTAuthServer.Data");
                });
            });

            // Add services to the container.
            builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));
            builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions!.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // register validators
            builder.Services.AddFluentValidationAutoValidation(); // the same old MVC pipeline behavior
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.UseCustomValidationResponse();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCustomException();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
