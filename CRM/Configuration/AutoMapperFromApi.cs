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
            CreateMap<AccountInsertRequest, AccountModel>();
            CreateMap<TransactionRequest, TransactionModel>();

            CreateMap<LeadModel, LeadResponse>();
            CreateMap<AccountModel, AccountResponse>();
        }
    }
}
