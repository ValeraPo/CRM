using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{ 
    public class AccountUpdateRequest
    {
        [StringLength(20, ErrorMessage = "Максимальная длина 20 символов.")]
        public string Name { get; set; }
    }
}
