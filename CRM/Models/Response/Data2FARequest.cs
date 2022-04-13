namespace CRM.APILayer.Models.Response
{
    public class Data2FARequest
    {
        public string LeadId { get; set; }
        public string EncodedKey { get; set; }
    }
}
