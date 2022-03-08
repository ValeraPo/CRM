using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{ 
    public class AccountInsertRequest : AccountUpdateRequest
    {
        [Required(ErrorMessage = "Поле CurrencyType не может быть пустым")]
        public string CurrencyType { get; set; }
    }
}
