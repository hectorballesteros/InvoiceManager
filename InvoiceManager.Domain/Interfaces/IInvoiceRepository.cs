using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceManager.Domain.Entities;
using System.Linq; 

namespace InvoiceManager.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task SaveAsync();
        Task AddInvoiceAsync(Invoice invoice); // Método para agregar una nueva factura
        Task<List<Invoice>> GetAllInvoicesAsync(); // Obtener todas las facturas
        Task<Invoice> GetInvoiceByNumberAsync(int invoiceNumber); // Obtener factura por número
        Task<Customer> GetCustomerByRunAsync(string customerRun); // Obtener cliente por RUN
        Task AddCustomerAsync(Customer customer); // Agregar un cliente nuevo
        Task<List<Invoice>> GetInvoicesByStatusAsync(string? invoiceStatus, string paymentStatus);  // Filtrar por PaymentStatus como string
        Task AddCreditNoteAsync(CreditNote creditNote); // Agregar una nueva nota de crédito
    }
}
