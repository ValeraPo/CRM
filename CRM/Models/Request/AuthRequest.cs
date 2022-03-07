using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class AuthRequest
    {
        [EmailAddress(ErrorMessage = "Email введен некорректно.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
