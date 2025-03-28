using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice); // Método para agregar una nueva factura
/*         Task<Invoice> GetByIdAsync(int id); // Obtener una factura por su ID
        Task<IEnumerable<Invoice>> GetAllAsync(); // Obtener todas las facturas
        Task UpdateAsync(Invoice invoice); // Actualizar una factura existente
        Task DeleteAsync(int id); // Eliminar una factura por su ID */
    }
}
