using CRM_APILayer.Configurations;
using CRM_BuisnessLayer.Configurations;
using CRM_BuisnessLayer.Services;
using CRM_BuisnessLayer.Services.Interfaces;
using CRM_DataLayer.Repositories;
using CRM_DataLayer.Repositories.Interfaces;
using AutoMapper;

namespace CRM_APILayer.Extensions
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
