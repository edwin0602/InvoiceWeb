using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class InvoicikaDbContext : DbContext
    {
        public InvoicikaDbContext(DbContextOptions<InvoicikaDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerInvoice> CustomerInvoices { get; set; }
        public DbSet<CustomerInvoiceLine> CustomerInvoiceLines { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VAT> VATs { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CustomerFile> CustomerFiles { get; set; }
        public DbSet<CustomerInvoiceFile> CustomerInvoiceFiles { get; set; }
        public DbSet<CustomerInvoiceNote> CustomerInvoiceNotes { get; set; }
        public DbSet<CustomerInvoicePay> CustomerInvoicePays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerFile>()
                .HasOne(cf => cf.Customer)
                .WithMany(c => c.CustomerFiles)
                .HasForeignKey(cf => cf.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerInvoiceFile>()
                .HasOne(cif => cif.CustomerInvoice)
                .WithMany(ci => ci.CustomerInvoiceFiles)
                .HasForeignKey(cif => cif.CustomerInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerInvoiceNote>()
                .HasOne(cif => cif.CustomerInvoice)
                .WithMany(ci => ci.CustomerInvoiceNotes)
                .HasForeignKey(cif => cif.CustomerInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerInvoicePay>()
                .HasOne(cil => cil.CustomerInvoice)
                .WithMany(i => i.CustomerInvoicePays)
                .HasForeignKey(cil => cil.CustomerInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerInvoiceLine>()
                .HasOne(cil => cil.Item)
                .WithMany(i => i.CustomerInvoiceLines)
                .HasForeignKey(cil => cil.Item_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
