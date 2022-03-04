using CRM.DataLayer;
using System.ComponentModel.DataAnnotations;

namespace CRM_APILayer.Models
{
    public class LeadUpdateInputModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateBirth { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
    }
}
