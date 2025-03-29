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
                // Verificar si la factura ya existe por su número
                if (await IsInvoiceExisting(invoice.InvoiceNumber))
                {
                    // Si la factura ya existe, omitimos la factura y agregamos un error
                    errors.Add($"Se omitió la factura con número {invoice.InvoiceNumber} porque ya existe una factura con ese número.");
                    continue;
                }

                // Buscar si el cliente ya existe
                if (invoice.Customer != null)
                {
                    var existingCustomer = await IsCustomerExisting(invoice.Customer.CustomerRun);

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

        public async Task<Invoice> GetInvoiceByNumber(int invoiceNumber)
        {
            return await _invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);
        }

        public async Task<List<Invoice>> GetInvoicesByStatusAsync(string? invoiceStatus, string? paymentStatus)
        {
            return await _invoiceRepository.GetInvoicesByStatusAsync(invoiceStatus, paymentStatus);
        }

        public async Task<ImportResult> AddCreditNoteToInvoice(int invoiceNumber, decimal creditNoteAmount)
        {
            var errors = new List<string>();

            var invoice = await _invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);

            if (invoice == null)
            {
                errors.Add("Factura no encontrada.");
                return new ImportResult
                {
                    Success = false,
                    Message = "No se encontró la factura con el número especificado.",
                    Errors = errors
                };
            }

            // Calcular notas de crédito actuales
            var totalCredited = invoice.InvoiceCreditNote.Sum(c => c.CreditNoteAmount);
            var remainingBalance = invoice.TotalAmount - totalCredited;

            // Validar que la nueva NC no exceda lo disponible
            if (creditNoteAmount > remainingBalance)
            {
                errors.Add($"El monto de la nota de crédito ({creditNoteAmount}) excede el saldo pendiente ({remainingBalance}).");
                return new ImportResult
                {
                    Success = false,
                    Message = "La nota de crédito excede el saldo pendiente de la factura.",
                    Errors = errors
                };
            }

            // Crear y agregar la nota de crédito
            var creditNote = new CreditNote
            {
                InvoiceId = invoice.Id,
                CreditNoteAmount = creditNoteAmount,
                CreditNoteDate = DateTime.Now,
                CreditNoteNumber = new Random().Next(1000, 9999)
            };

            await _invoiceRepository.AddCreditNoteAsync(creditNote);

            // Recalcular total acreditado incluyendo la nueva NC
            var updatedTotalCredited = totalCredited + creditNoteAmount;
            var updatedRemaining = invoice.TotalAmount - updatedTotalCredited;

            // Actualizar estado
            invoice.InvoiceStatus = updatedTotalCredited switch
            {
                var t when t == invoice.TotalAmount => "Cancelled",
                var t when t > 0 => "Partial",
                _ => "Pending"
            };

            await _invoiceRepository.SaveAsync();

            return new ImportResult
            {
                Success = true,
                Message = $"Nota de crédito de {creditNoteAmount:C} agregada correctamente. Saldo restante: {updatedRemaining:C}.",
                Errors = errors
            };
        }











        // Verificar si la factura ya existe por su número
        private async Task<bool> IsInvoiceExisting(int invoiceNumber)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);
            return existingInvoice != null;
        }

        // Buscar si el cliente ya existe
        private async Task<Customer> IsCustomerExisting(string customerRun)
        {
            return await _invoiceRepository.GetCustomerByRunAsync(customerRun);
        }

        // Validación del subtotal de la factura
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
