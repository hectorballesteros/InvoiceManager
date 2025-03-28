using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace InvoiceManager.Domain.Entities
{
    public class InvoiceDetail
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        // Llave foránea hacia Invoice
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public Invoice Invoice { get; set; }
    }
}
