using InvoiceManager.Domain.Entities;
using InvoiceManager.Domain.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InvoiceManager.Application.Services
{


    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }

    
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
         private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository invoiceRepository, ILogger<InvoiceService> logger)
        {
            _invoiceRepository = invoiceRepository;
             _logger = logger;
        }


        public async Task<List<Invoice>> GetAllInvoices()
        {
            return await _invoiceRepository.GetAllAsync();
        }


    public async Task<ImportResult> ImportInvoices(List<Invoice> invoices)
    {
        var validInvoices = new List<Invoice>();
        var errors = new List<string>();

        foreach (var invoice in invoices)
        {
            // Primero, buscar si el cliente ya existe
            if (invoice.Customer != null)
            {
                var existingCustomer = await _invoiceRepository.GetCustomerByRunAsync(invoice.Customer.CustomerRun);
                
                if (existingCustomer != null)
                {
                    // Si el cliente ya existe, asignamos el cliente existente
                    invoice.Customer = existingCustomer;
                }
                else
                {
                    // Si el cliente no existe, lo creamos
                    await _invoiceRepository.AddCustomerAsync(invoice.Customer);
                }
            }

            if (!IsInvoiceSubtotalValid(invoice, out var error))
            {
                errors.Add(error);
                continue;
            }

            invoice.InvoiceStatus = CalculateInvoiceStatus(invoice);
            invoice.PaymentStatus = CalculatePaymentStatus(invoice);

            if (invoice.Payment != null && string.IsNullOrEmpty(invoice.Payment.PaymentMethod))
            {
                invoice.Payment = null;
            }

            validInvoices.Add(invoice);
        }

        foreach (var invoice in validInvoices)
        {
            await _invoiceRepository.AddAsync(invoice);
        }

        string successMessage = validInvoices.Count > 0
            ? $"Se importaron correctamente {validInvoices.Count} factura(s)."
            : "No se importó ninguna factura.";

        return new ImportResult
        {
            Success = true,
            Message = successMessage,
            Errors = errors
        };
    }




        private bool IsInvoiceSubtotalValid(Invoice invoice, out string errorMessage)
        {
            decimal calculatedSubtotal = invoice.InvoiceDetail.Sum(d => d.Subtotal);

            if (invoice.TotalAmount != calculatedSubtotal)
            {
                errorMessage = $"Se omitió la factura {invoice.InvoiceNumber} porque el total no coincide con los subtotales.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private string CalculateInvoiceStatus(Invoice invoice)
        {
            if (invoice.InvoiceCreditNote != null && invoice.InvoiceCreditNote.Count > 0)
            {
                decimal totalCreditNoteAmount = 0;
                foreach (var creditNote in invoice.InvoiceCreditNote)
                {
                    totalCreditNoteAmount += creditNote.CreditNoteAmount;
                }

                if (totalCreditNoteAmount >= invoice.TotalAmount)
                    return "Cancelled";
                if (totalCreditNoteAmount > 0)
                    return "Partial";
            }

            return "Issued";
        }

        private string CalculatePaymentStatus(Invoice invoice)
        {
            if (invoice.PaymentStatus == "Paid")
                return "Paid";

            if (invoice.PaymentDueDate < DateTime.Now)
                return "Overdue";

            return "Pending";
        }
    }
}
