using AutoMapper;
using CRM_APILayer.Models;
using CRM_BuisnessLayer.Models;

namespace CRM_APILayer.Configurations
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
