using CRM.APILayer.Configuration;
using CRM.APILayer.Consumers;
using CRM.APILayer.Producers;
using CRM.APILayer.Validation;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Repositories;
using CRM.DataLayer.Repositories.Interfaces;
using FluentValidation.AspNetCore;
using Marvelous.Contracts.Client;
using MassTransit;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;


namespace CRM.APILayer.Extensions
{
    public static class IServiceProviderExtensioncs
    {
        // Registration repositories
        public static void RegisterCRMRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
        }

        // Registration services
        public static void RegisterCRMServices(this IServiceCollection services)
        {
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IRequestHelper, RequestHelper>();
            services.AddScoped<ICRMProducers, CRMProducer>();
            services.AddTransient<IInitializationHelper, InitializationHelper>();
            services.AddScoped<IRestClient, MarvelousRestClient>();

        }

        // Validation
        public static void AddFluentValidation(this IServiceCollection services)
        {
            //Adding FluentValidation
            services.AddFluentValidation(fv =>
            {
                //Register validators  Singleton
                fv.RegisterValidatorsFromAssemblyContaining<LeadInsertRequestValidator>(lifetime: ServiceLifetime.Singleton);
                //Turning off validation with DataAnnotations
                fv.DisableDataAnnotationsValidation = true;
            });
            //Turning off normal validation
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        }

        //Registration automappers
        public static void RegisterCRMAutomappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperFromApi), typeof(AutoMapperToData));
        }

        // Adding authorization
        public static void AddCustomAuth(this IServiceCollection services)
        {
            // Turn off validate token, because we chek in in authorisation service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                    };
                });
            services.AddAuthorization();
        }

        // Settings swagger
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

        // Adding logger
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

        // Addind MassTransit (RabbitMQ)
        public static void AddMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<LeadConsumer>();
                x.AddConsumer<ConfigConsumer>();
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
                    cfg.ReceiveEndpoint("ChangeConfigCrm", e =>
                    {
                        e.PurgeOnStartup = true;
                        e.ConfigureConsumer<ConfigConsumer>(context);
                    });
                });
            });
        }

    }
}
