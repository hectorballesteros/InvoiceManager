using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace InvoiceManager.Domain.Entities
{
    public class Payment
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_date")]
        public DateTime? PaymentDate { get; set; }

        // Llave foránea hacia Invoice
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public Invoice Invoice { get; set; }
    }
}
