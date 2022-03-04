using CRM.DataLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadInsertInputModel : LeadInputModel
    {
        [EmailAddress(ErrorMessage = "Email введен некорректно.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Поле должно иметь минимум 8 и максимум 30 символов.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
