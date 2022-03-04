namespace CRM_DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        void AddLead(Lead lead);
        void UpdateLeadById(Lead lead);
    }
}
