// InvoiceManager.Api/Models/InvoiceImportRequest.cs
using InvoiceManager.Domain.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace InvoiceManager.Api.Models
{
    public class InvoiceImportRequest
    {
        [JsonPropertyName("invoices")]
        public List<Invoice> Invoices { get; set; }
    }
}
