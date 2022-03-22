using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        Task<int> AddLead(LeadModel leadModel);
        Task UpdateLead(int id, LeadModel leadModel);
        Task ChangeRoleLead(int id, int role);
        Task DeleteById(int id);
        Task RestoreById(int id);
        Task ChangePassword(int id, string oldPassword, string newPassword);
        Task<List<LeadModel>> GetAll();
        Task<LeadModel> GetById(int id);
    }
}
