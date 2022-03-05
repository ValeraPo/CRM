using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;

namespace CRM.APILayer.Configuration
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
