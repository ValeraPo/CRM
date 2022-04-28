namespace CRM.APILayer.Models
{
    public class TransferShortRequest
    {
        public decimal Amount{get;set;}
        public int AccountIdFrom{get;set;}
        public int AccountIdTo{get;set;}
    }
}
