using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InvoiceManager.Domain.Entities
{
    public class Invoice
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("invoice_number")]
        public int InvoiceNumber { get; set; }

        [JsonProperty("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [JsonProperty("invoice_status")]
        public string InvoiceStatus { get; set; }
        
        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("days_to_due")]
        public int DaysToDue { get; set; }

        [JsonProperty("payment_due_date")]
        public DateTime PaymentDueDate { get; set; }

        [JsonProperty("payment_status")]
        public string PaymentStatus { get; set; }

        [JsonProperty("invoice_detail")]
        public List<InvoiceDetail> InvoiceDetail { get; set; }

        [JsonProperty("invoice_payment")]
        public InvoicePayment InvoicePayment { get; set; }

        [JsonProperty("invoice_credit_note")]
        public List<CreditNote> InvoiceCreditNote { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        public Invoice()
        {
            InvoiceDetail = new List<InvoiceDetail>();
            InvoiceCreditNote = new List<CreditNote>();
        }
    }

    public class InvoiceDetail
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("subtotal")]
        public decimal Subtotal { get; set; }
    }

    public class InvoicePayment
    {
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payment_date")]
        public DateTime? PaymentDate { get; set; }
    }

    public class CreditNote
    {
        [JsonProperty("credit_note_number")]
        public int CreditNoteNumber { get; set; }

        [JsonProperty("credit_note_date")]
        public DateTime CreditNoteDate { get; set; }

        [JsonProperty("credit_note_amount")]
        public decimal CreditNoteAmount { get; set; }
    }

    public class Customer
    {
        [JsonProperty("customer_run")]
        public string CustomerRun { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }
    }
}
