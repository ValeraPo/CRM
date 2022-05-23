using AutoMapper;
using CRM.BusinessLayer.Models;
using CRM.DataLayer.Entities;


namespace CRM.BusinessLayer.Configurations
{
    // Mapping repositories and models from BLL
    public class AutoMapperToData : Profile
    {
        public AutoMapperToData()
        {
            CreateMap<Lead, LeadModel>().ReverseMap();
            CreateMap<Account, AccountModel>().ReverseMap();
        }
    }
}
