using CRM.DataLayer;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateBirth { get; set; }

        [EmailAddress(ErrorMessage = "Email введен некорректно.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
        public Enum Role { get; set; }
    }
}
