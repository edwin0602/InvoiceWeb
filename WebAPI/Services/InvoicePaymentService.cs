using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Constants;
using WebAPI.Data;
using WebAPI.Dtos;
using WebAPI.Helpers;

namespace WebAPI.Services
{
    public interface IInvoicePaymentService
    {
        Task<IEnumerable<InvoicePaymentDto>> GetInvoicePaymentsAsync();
        Task<InvoicePaymentDto> GetInvoicePaymentByIdAsync(Guid id);
        Task AddInvoicePaymentAsync(InvoicePaymentDto customerInvoicePay);
        Task UpdateInvoicePaymentAsync(InvoicePaymentDto customerInvoicePay);
        Task MarkAsDeleteInvoicePaymentAsync(Guid id);
    }

    public class InvoicePaymentService : IInvoicePaymentService
    {
        private readonly InvoicikaDbContext _context;

        public InvoicePaymentService(InvoicikaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoicePaymentDto>> GetInvoicePaymentsAsync()
        {
            var invoicePays = await _context.CustomerInvoicePays
                .Include(p => p.User)
                .ToListAsync();

            return invoicePays.Select(CustomerInvoiceMapper.ToDto);
        }

        public async Task<InvoicePaymentDto> GetInvoicePaymentByIdAsync(Guid id)
        {
            var payment = await _context.CustomerInvoicePays
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.CustomerInvoicePaymentId == id)
                ?? throw new KeyNotFoundException("Customer Invoice Payment not found");

            return CustomerInvoiceMapper.ToDto(payment);
        }

        public async Task AddInvoicePaymentAsync(InvoicePaymentDto customerInvoicePay)
        {
            if (customerInvoicePay.Amount <= 0)
                throw new ArgumentException("El monto del pago debe ser mayor a cero.");

            var invoice = await _context.CustomerInvoices
                .FirstOrDefaultAsync(i => i.CustomerInvoiceId == customerInvoicePay.CustomerInvoiceId)
                ?? throw new KeyNotFoundException("Customer Invoice not found");

            var invoicePay = CustomerInvoiceMapper.ToModel(customerInvoicePay);
            invoicePay.Status = GeneralStatuses.Active;

            _context.CustomerInvoicePays.Add(invoicePay);

            invoice.PaidAmount += customerInvoicePay.Amount;
            invoice.Status = InvoiceStatuses.PartiallyPaid;

            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = InvoiceStatuses.Paid;

            _context.CustomerInvoices.Update(invoice);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvoicePaymentAsync(InvoicePaymentDto customerInvoicePay)
        {
            _ = await _context.CustomerInvoicePays.FirstOrDefaultAsync(p => p.CustomerInvoicePaymentId == customerInvoicePay.CustomerInvoicePayId)
                ?? throw new KeyNotFoundException("Customer Invoice Payment not found");

            var invoicePay = CustomerInvoiceMapper.ToModel(customerInvoicePay);
            _context.CustomerInvoicePays.Update(invoicePay);

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsDeleteInvoicePaymentAsync(Guid id)
        {
            var customerInvoicePay = await _context.CustomerInvoicePays.FirstOrDefaultAsync(p => p.CustomerInvoicePaymentId == id)
                ?? throw new KeyNotFoundException("Customer Invoice Payment not found");

            customerInvoicePay.Status = GeneralStatuses.Deleted;
            _context.CustomerInvoicePays.Update(customerInvoicePay);

            await _context.SaveChangesAsync();
        }
    }
}
