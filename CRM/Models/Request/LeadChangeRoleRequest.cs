using Marvelous.Contracts;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadChangeRoleRequest : LeadRequest
    {
        public int Id { get; set; }

        [Range(2, 3, ErrorMessage = "Роль может быть 2 - Vip или 3 - Regular.")]
        public Role Role { get; set; }
    }
}
