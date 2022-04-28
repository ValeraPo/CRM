namespace CRM.APILayer.Models
{
    public class LeadChangePasswordRequest 
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
