using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Models.Paypal.Enums
{
    public enum InvoiceStatus
    {
        DRAFT,
        SENT,
        SCHEDULED,
        PAID,
        MARKED_AS_PAID,
        CANCELLED,
        REFUNDED,
        PARTIALLY_PAID,
        PARTIALLY_REFUNDED,
        MARKED_AS_REFUNDED,
        UNPAID,
        PAYMENT_PENDING
    }
}
