using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InvoiceManager.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InvoiceManagerContext>
    {
        public InvoiceManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InvoiceManagerContext>();

            // Configuración de la cadena de conexión directamente en el código
            var connectionString = "Data Source=invoice_manager.db"; 
            // Configurar el DbContext con SQLite usando la cadena de conexión directamente
            optionsBuilder.UseSqlite(connectionString);

            return new InvoiceManagerContext(optionsBuilder.Options);
        }
    }
}
