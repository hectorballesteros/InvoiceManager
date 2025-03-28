using InvoiceManager.Domain.Interfaces;
using InvoiceManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManager.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly List<Invoice> _invoices = new List<Invoice>();

        public async Task AddAsync(Invoice invoice)
        {
            _invoices.Add(invoice);
            await Task.CompletedTask; // Simulando una operación asíncrona
        }

        public List<Invoice> GetAll() => _invoices;
    }
}
