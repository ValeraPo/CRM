using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadInputModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
    }
}
