using AutoMapper;
using CRM_APILayer.Models;
using CRM_BuisnessLayer.Models;

namespace CRM_APILayer.Configuration
{
    public class AutoMapperFromApi : Profile
    {
        public AutoMapperFromApi()
        {
            CreateMap<LeadInsertInputModel, LeadModel>();
            CreateMap<LeadUpdateInputModel, LeadModel>();
            CreateMap<LeadModel, LeadOutputModel>();
            CreateMap<AccountInputModel, AccountModel>();
            CreateMap<AccountModel, AccountOutputModel>();
        }
    }
}
