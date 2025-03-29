using InvoiceManager.Domain.Interfaces;
using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManager.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceManagerContext _context;

        // Constructor que inyecta el contexto
        public InvoiceRepository(InvoiceManagerContext context)
        {
            _context = context;
        }

        // Agregar una factura asincrónicamente
        public async Task AddAsync(Invoice invoice)
        {
            // Agregar la factura al contexto de EF Core
            await _context.Invoices.AddAsync(invoice);
            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }

        // Obtener todas las facturas
        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetail)
                .Include(i => i.Customer)
                .Include(i => i.Payment)
                .Include(i => i.InvoiceCreditNote)
                .ToListAsync();
        }


        // Obtener una factura por su Id
        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice> GetInvoiceByNumberAsync(int invoiceNumber)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceCreditNote) 
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }


        public async Task<Customer> GetCustomerByRun(string customerRun)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerRun == customerRun);
        }

        public async Task<Customer> GetCustomerByRunAsync(string customerRun)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerRun == customerRun);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Invoice>> GetInvoicesByStatusAsync(string? invoiceStatus, string paymentStatus)
        {
            IQueryable<Invoice> query = _context.Invoices;

            // Filtrar por estado de la factura si se proporciona
            if (!string.IsNullOrEmpty(invoiceStatus))
            {
                query = query.Where(i => i.InvoiceStatus == invoiceStatus);
            }

            // Filtrar por estado de pago si se proporciona
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(i => i.PaymentStatus == paymentStatus);  // Filtro por PaymentStatus
            }

            return await query.Include(i => i.Customer) // Incluir información del cliente
                              .ToListAsync();
        }

        public async Task AddCreditNoteAsync(CreditNote creditNote)
        {
            await _context.CreditNotes.AddAsync(creditNote); // Agregar la NC a la tabla CreditNotes
            await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();  // Guarda todos los cambios pendientes en la base de datos
        }





        
    }
}
