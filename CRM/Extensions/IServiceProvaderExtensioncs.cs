using CRM.APILayer.Configuration;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using AutoMapper;
using CRM.DataLayer.Repositories;
using CRM.DataLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BearGoodbyeKolkhozProject.Business.Configuration;
using Microsoft.OpenApi.Models;

namespace CRM.APILayer.Extensions
{
    public static class IServiceProvaderExtensioncs
    {
        public static void RegisterCRMRepositories(this IServiceCollection services)
        {


            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            
        }

        public static void RegisterCRMServices(this IServiceCollection services)
        {
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IAccountService, AccountService>();
        }

        public static void RegisterCRMAutomappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperFromApi), typeof(AutoMapperToData));
        }

        public static void AddCustomAuth(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });
            services.AddAuthorization();
        }

        public static void RegisterSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."

                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });

                //config.EnableAnnotations();
            });
        }


    }
}
