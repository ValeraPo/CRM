using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        int AddLead(LeadModel leadModel);
        void UpdateLead(int id, LeadModel leadModel);
        void ChangeRoleLead(int id, int role);
        void DeleteById(int id);
        void RestoreById(int id);
        void ChangePassword(int id, string oldPassword, string newPassword);
        List<LeadModel> GetAll();
        LeadModel GetById(int id);
    }
}
