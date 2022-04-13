using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyName("unit_amount")]
        public UnitAmount UnitAmount { get; set; }
        [JsonPropertyName("unit_of_measure")]
        public string UnitOfMeasure { get; set; }
    }
}
