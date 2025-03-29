using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace InvoiceManager.Domain.Entities
{
    public class Invoice
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("invoice_number")]
        public int InvoiceNumber { get; set; }

        [JsonPropertyName("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [JsonPropertyName("invoice_status")]
        public string InvoiceStatus { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("days_to_due")]
        public int DaysToDue { get; set; }

        [JsonPropertyName("payment_due_date")]
        public DateTime PaymentDueDate { get; set; }

        [JsonPropertyName("payment_status")]
        public string PaymentStatus { get; set; }

        // Relación con detalles de la factura
        [JsonPropertyName("invoice_detail")]
        public List<InvoiceDetail> InvoiceDetail { get; set; }

        // Relación con el pago de la factura
        [JsonPropertyName("invoice_payment")]
        public Payment Payment { get; set; }

        // Relación con las notas de crédito
        [JsonPropertyName("invoice_credit_note")]
        public List<CreditNote> InvoiceCreditNote { get; set; }

        // Relación con el cliente (llave foránea)
        [ForeignKey("CustomerId")]
        [JsonPropertyName("customer")]
        public Customer Customer { get; set; }

        [JsonPropertyName("customer_id")]
        public int CustomerId { get; set; }

        public Invoice()
        {
            InvoiceDetail = new List<InvoiceDetail>();
            InvoiceCreditNote = new List<CreditNote>();
        }
    }
}

