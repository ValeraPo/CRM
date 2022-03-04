using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadUpdateInputModel : LeadInputModel
    {
        [Range(1, 3, ErrorMessage = "Роль должна быть в диапазоне от 1 до 3")]
        public int Role { get; set; }
    }
}
