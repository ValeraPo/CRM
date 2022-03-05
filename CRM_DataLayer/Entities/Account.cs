namespace CRM.DataLayer.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrencyType { get; set; }
        public Lead Lead { get; set; }
    }
}