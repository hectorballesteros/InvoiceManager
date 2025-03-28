// InvoiceManager.Api/Models/InvoiceImportRequest.cs
using InvoiceManager.Domain.Entities;
using System.Collections.Generic;

namespace InvoiceManager.Api.Models
{
    public class InvoiceImportRequest
    {
        public List<Invoice> Invoices { get; set; }
    }
}
