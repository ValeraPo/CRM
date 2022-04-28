namespace CRM.APILayer.Models
{
    public class LeadUpdateRequest 
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
    }
}
