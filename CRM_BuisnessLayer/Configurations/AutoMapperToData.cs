using AutoMapper;
using CRM.DataLayer.Entities;
using CRM_BuisnessLayer.Models;


namespace CRM_BuisnessLayer.Configurations
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
