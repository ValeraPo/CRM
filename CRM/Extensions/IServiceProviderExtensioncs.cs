using CRM.APILayer.Configuration;
using CRM.APILayer.Consumer;
using CRM.APILayer.Producers;
using CRM.APILayer.Validation;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Repositories;
using CRM.DataLayer.Repositories.Interfaces;
using FluentValidation.AspNetCore;
using Marvelous.Contracts.ExchangeModels;
using MassTransit;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
//using MicroElements.Swashbuckle.FluentValidation.AspNetCore;


namespace CRM.APILayer.Extensions
{
    public static class IServiceProviderExtensioncs
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
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IRequestHelper, RequestHelper>();
            services.AddScoped<ICRMProducers, CRMProducer>();
            services.AddTransient<IInitializationHelper, InitializationHelper>();
        }

        public static void AddFluentValidation(this IServiceCollection services)
        {
            //Добавление FluentValidation
            services.AddFluentValidation(fv =>
            {
                //Регистрация валидаторов по сборке с временем жизни = Singleton
                fv.RegisterValidatorsFromAssemblyContaining<LeadInsertRequestValidator>(lifetime: ServiceLifetime.Singleton);
                //Отключение валидации с помощью DataAnnotations
                fv.DisableDataAnnotationsValidation = true;
            });
            //Отключение стандартного валидатора
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
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
                        ValidateIssuer = false,
                        //ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = false,
                        //ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = false,
                        //IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        //ValidateIssuerSigningKey = true
                    };
                });
            services.AddAuthorization();
        }

        public static void RegisterSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.EnableAnnotations();
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyAPI",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Git Repository",
                        Url = new Uri("https://github.com/ValeraPo/CRM"),
                    }
                });

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
            });
            services.AddFluentValidationRulesToSwagger();

        }

        public static void RegisterLogger(this IServiceCollection service, IConfiguration config)
        {
            service.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
            service.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                loggingBuilder.AddNLog(config);
            });
        }

        public static void AddMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<LeadConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://80.78.240.16", hst =>
                    {
                        hst.Username("nafanya");
                        hst.Password("qwe!23");
                    });
                    cfg.ReceiveEndpoint("leadCRMQueue", e =>
                    {
                        e.ConfigureConsumer<LeadConsumer>(context);
                    });
                    cfg.Publish<LeadFullExchangeModel>(p =>
                    {
                        p.BindAlternateExchangeQueue("alternate-exchange", "alternate-queue");
                    });
                    cfg.Publish<AccountExchangeModel>(p =>
                    {
                        p.BindAlternateExchangeQueue("alternate-exchange", "alternate-queue");
                    });
                });
            });
        }

    }
}
