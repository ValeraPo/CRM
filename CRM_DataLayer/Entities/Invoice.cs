using CRM.DataLayer.Enums;

namespace CRM.DataLayer.Entities
{
    public class Invoice
    {
        public string Id { get; set; }
        public Account Account { get; set; }
        public decimal Amount { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
