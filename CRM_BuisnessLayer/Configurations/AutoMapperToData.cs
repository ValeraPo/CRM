using AutoMapper;
using CRM_BuisnessLayer.Models;
using CRM_DataLayer;


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
