using CRM.DataLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace CRM_APILayer.Models
{
    public class LeadInsertInputModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        [EmailAddress(ErrorMessage = "Email введен некорректно.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
