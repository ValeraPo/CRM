using CRM.BusinessLayer.Models;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.ResponseModels;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        Task<(int, int)> AddLead(LeadModel leadModel);
        Task UpdateLead(int id, LeadModel leadModel);
        Task ChangeRoleLead(int id, Role role);
        Task DeleteById(int id);
        Task RestoreById(int id);
        Task ChangePassword(int id, string oldPassword, string newPassword);
        Task<List<LeadModel>> GetAll();
        Task<LeadModel> GetById(int id, IdentityResponseModel leadIdentity);
        Task<LeadModel> GetById(int id);
        Task<List<LeadAuthExchangeModel>> GetAllToAuth();
        Task ChangeRoleListLead(LeadShortExchangeModel[] models);
    }
}
