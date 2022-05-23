using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using Marvelous.Contracts.RequestModels;

namespace CRM.APILayer.Configuration
{
    // Mapping Request/Response Models and Model from BLL
    public class AutoMapperFromApi : Profile
    {
        public AutoMapperFromApi()
        {
            CreateMap<LeadInsertRequest, LeadModel>();
            CreateMap<LeadUpdateRequest, LeadModel>();
            CreateMap<AccountInsertRequest, AccountModel>();
            CreateMap<AccountModel, AccountInsertRequest>();

            CreateMap<LeadModel, LeadResponse>();
            CreateMap<AccountModel, AccountResponse>();
            CreateMap<AccountModel, AccountShortResponse>();
            CreateMap<AccountUpdateRequest, AccountModel>();

            CreateMap<TransferShortRequest, TransferRequestModel>();
            CreateMap<TransactionShortRequest, TransactionRequestModel>();
        }
    }
}
