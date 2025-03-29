using Microsoft.EntityFrameworkCore;
using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Infrastructure
{
    public class InvoiceManagerContext : DbContext
    {
        public InvoiceManagerContext(DbContextOptions<InvoiceManagerContext> options)
            : base(options)
        {
        }

        // Definir las tablas en la base de datos
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }

        // Configuración de relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de las relaciones
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId);

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Invoice)
                .WithMany(i => i.InvoiceDetail)
                .HasForeignKey(id => id.InvoiceId);

            modelBuilder.Entity<Payment>()
                .HasOne(ip => ip.Invoice)
                .WithOne(i => i.Payment)
                .HasForeignKey<Payment>(ip => ip.InvoiceId);

            modelBuilder.Entity<CreditNote>()
                .HasOne(cn => cn.Invoice)
                .WithMany(i => i.InvoiceCreditNote)
                .HasForeignKey(cn => cn.InvoiceId);
        }
    }
}
