using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BusinessLayer.Models
{
    public class Data2FAModel
    {
        public string LeadId { get; set; }
        public string EncodedKey { get; set; }
    }
}
