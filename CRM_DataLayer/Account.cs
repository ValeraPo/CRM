namespace CRM_DataLayer
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Balance { get; set; }
        public string CurrencyType { get; set; }
        public Lead Lead { get; set; }
    }
}