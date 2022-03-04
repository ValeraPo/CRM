using CRM.APILayer.Configuration;
using CRM.BusinessLayer.Configurations;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using AutoMapper;
using CRM.DataLayer.Repositories;
using CRM.DataLayer.Repositories.Interfaces;

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

    }
}
