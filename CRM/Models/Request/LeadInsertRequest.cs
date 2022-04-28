namespace CRM.APILayer.Models
{
    public class LeadInsertRequest : LeadUpdateRequest
    {

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
