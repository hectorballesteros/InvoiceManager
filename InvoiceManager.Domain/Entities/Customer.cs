using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Domain.Entities
{
    public class Customer
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("customer_run")]
        public string CustomerRun { get; set; }

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; }

        // Relación 1:N con Invoices
        [JsonIgnore]
        public List<Invoice> Invoices { get; set; }
    }
}
