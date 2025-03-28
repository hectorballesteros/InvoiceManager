using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;



namespace InvoiceManager.Domain.Entities
{
    public class CreditNote
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("credit_note_number")]
        public int CreditNoteNumber { get; set; }

        [JsonPropertyName("credit_note_date")]
        public DateTime CreditNoteDate { get; set; }

        [JsonPropertyName("credit_note_amount")]
        public decimal CreditNoteAmount { get; set; }

        // Llave foránea hacia Invoice
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public Invoice Invoice { get; set; }

    }
}
