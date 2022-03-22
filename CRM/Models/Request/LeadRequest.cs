using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadRequest
    {
        [StringLength(30, ErrorMessage = "Максимальная длина 30 символов.")]
        public string Name { get; set; }

        [StringLength(30, ErrorMessage = "Максимальная длина 30 символов.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле BirthDate не может быть пустым.")]
        public DateTime BirthDate { get; set; }

        [Phone(ErrorMessage = "Телефон введен некорректно.")]
        public string Phone { get; set; }
    }
}
