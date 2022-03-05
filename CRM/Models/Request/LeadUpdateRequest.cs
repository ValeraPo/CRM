using CRM.DataLayer;
using CRM.DataLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadUpdateRequest : LeadRequest
    {
        public int Id { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }

        [Range(1,3, ErrorMessage = "Роль должна быть в диапазоне от 1 до 3")]
        public Role Role { get; set; }
    }
}
