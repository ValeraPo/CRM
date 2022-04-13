using CRM.BusinessLayer.Models.Paypal;
using CRM.BusinessLayer.Models.Paypal.Request;

namespace CRM.BusinessLayer.Helpers
{
    public interface IPaypalRequestHelper
    {
        Task<Link> CreateDraftInvoice(InvoiceRequest request);
        Task<InvoiceResponse> GetInvoice(string invoiceId);
        Task<InvoiceNumberResponse> GetNextInvoiceNumber();
        Task<ListInvoicesResponse> ListInvoices();
        Task<Link> SendInvoice(string invoiceId, SendInvoiceRequest request);
        string GetInvoiceIdFromLink(Link link);
    }
}