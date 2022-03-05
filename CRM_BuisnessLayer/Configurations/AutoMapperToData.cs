using AutoMapper;
using CRM.DataLayer.Entities;
using CRM.BusinessLayer.Models;


namespace CRM.BusinessLayer.Configurations
{
    public class AutoMapperToData: Profile
    {
        public AutoMapperToData()
        {
            CreateMap<Lead, LeadModel>().ReverseMap();
            CreateMap<Account, AccountModel>().ReverseMap();
        }
    }
}
