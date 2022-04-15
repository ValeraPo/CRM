using CRM.DataLayer.Entities;
using CRM.DataLayer.Enums;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<int> Add(Invoice invoice, int accountId);
        Task<List<Invoice>> GetAll();
        Task<Invoice> GetById(string id);
        Task UpdateStatus(string id, InvoiceStatus status);
    }
}