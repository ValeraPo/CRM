using CRM_DataLayer;
using System.ComponentModel.DataAnnotations;

namespace CRM_APILayer.Models
{
    public class LeadInsertRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateBirth { get; set; }

        [EmailAddress(ErrorMessage = "Email введен некорректно.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
