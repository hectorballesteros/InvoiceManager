using InvoiceManager.Domain.Entities;
using InvoiceManager.Domain.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManager.Application.Services
{


    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Invoice> Data { get; set; }
        public List<Invoice> InvalidInvoices { get; set; }
    }
    
    public class InvoiceImportService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceImportService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ImportResult> ImportInvoicesService(List<Invoice> invoices)
        {
            var validInvoices = new List<Invoice>();
            var invalidInvoices = new List<Invoice>();
            string errorMessage = "";

            // Validación de los datos de las facturas
            foreach (var invoice in invoices)
            {
                // Calcular el subtotal total de la factura sumando los subtotales de los detalles
                decimal calculatedSubtotal = 0;
                foreach (var detail in invoice.InvoiceDetail)
                {
                    calculatedSubtotal += detail.Subtotal;
                }

                // Verificar que la suma de los subtotales coincida con el total
                if (invoice.TotalAmount != calculatedSubtotal)
                {
                    invalidInvoices.Add(invoice);
                    continue; // Ignorar la factura con inconsistencias
                }

                // Calcular el estado de la factura (por ejemplo, "Issued", "Partial", "Cancelled")
                invoice.InvoiceStatus = CalculateInvoiceStatus(invoice);

                // Calcular el estado de pago (por ejemplo, "Pending", "Overdue", "Paid")
                invoice.PaymentStatus = CalculatePaymentStatus(invoice);

                // Agregar la factura válida a la lista de facturas válidas
                validInvoices.Add(invoice);
            }

            // Guardar solo las facturas válidas en la base de datos
            foreach (var invoice in validInvoices)
            {
                await _invoiceRepository.AddAsync(invoice);
            }

            // Construir el mensaje
            if (invalidInvoices.Count > 0)
            {
                errorMessage = $"Se encontraron {invalidInvoices.Count} facturas inconsistentes.";
            }

            return new ImportResult
            {
                Success = true,
                Message = errorMessage,
                InvalidInvoices = invalidInvoices,
                Data = validInvoices
            };
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
