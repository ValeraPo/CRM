using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadChangePasswordRequest
    {
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Пароль должен иметь длину минимум 8 и максимум 30 символов.", MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
