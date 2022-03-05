using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{ 
    public class AccountInputModel
    {
        [StringLength(20, ErrorMessage = "Максимальная длина 20 символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле CurrencyType не может быть пустым")]
        public string CurrencyType { get; set; }

        [Required(ErrorMessage = "Поле LeadId не может быть пустым")]
        public int LeadId { get; set; }
    }
}
