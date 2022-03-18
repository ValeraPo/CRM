using Marvelous.Contracts;
using System.ComponentModel.DataAnnotations;

namespace CRM.APILayer.Models
{
    public class LeadUpdateRequest : LeadRequest
    {
        public int Id { get; set; }

    }
}
