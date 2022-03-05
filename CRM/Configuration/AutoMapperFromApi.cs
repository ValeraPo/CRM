using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;

namespace CRM.APILayer.Configuration
{
    public class AutoMapperFromApi : Profile
    {
        public AutoMapperFromApi()
        {
            CreateMap<LeadInsertRequest, LeadModel>();
            CreateMap<LeadUpdateRequest, LeadModel>();
            CreateMap<LeadModel, LeadResponse>();
            CreateMap<AccountRequest, AccountModel>();
            CreateMap<AccountModel, AccountRespnse>();
        }
    }
}
