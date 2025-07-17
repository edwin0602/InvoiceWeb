using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Constants;
using WebAPI.Data;
using WebAPI.Dtos;
using WebAPI.Helpers;

namespace WebAPI.Services
{
    public interface ICustomerInvoicePayService
    {
        Task<IEnumerable<CustomerInvoicePayDto>> GetCustomerInvoicePaysAsync();
        Task<CustomerInvoicePayDto> GetCustomerInvoicePayByIdAsync(Guid id);
        Task AddCustomerInvoicePayAsync(CustomerInvoicePayDto customerInvoicePay);
        Task UpdateCustomerInvoicePayAsync(CustomerInvoicePayDto customerInvoicePay);
        Task MarkAsDeleteCustomerInvoicePayAsync(Guid id);
    }

    public class CustomerInvoicePayService: ICustomerInvoicePayService
    {
        private readonly InvoicikaDbContext _context;

        public CustomerInvoicePayService(InvoicikaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerInvoicePayDto>> GetCustomerInvoicePaysAsync()
        {
            var invoicePays = await _context.CustomerInvoicePays
                .Include(p => p.User)
                .ToListAsync();

            return invoicePays.Select(CustomerInvoiceMapper.ToDto);
        }

        public async Task<CustomerInvoicePayDto> GetCustomerInvoicePayByIdAsync(Guid id)
        {
            var pay = await _context.CustomerInvoicePays
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.CustomerInvoicePayId == id)
                ?? throw new KeyNotFoundException("Customer Invoice Pay not found");

            return CustomerInvoiceMapper.ToDto(pay);
        }

        public async Task AddCustomerInvoicePayAsync(CustomerInvoicePayDto customerInvoicePay)
        {
            _ = await _context.CustomerInvoices.FirstOrDefaultAsync(i => i.CustomerInvoiceId == customerInvoicePay.CustomerInvoiceId)
                ?? throw new KeyNotFoundException("Customer Invoice not found");

            var invoicePay = CustomerInvoiceMapper.ToModel(customerInvoicePay);
            invoicePay.Status = GeneralStatuses.Active;

            _context.CustomerInvoicePays.Add(invoicePay);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerInvoicePayAsync(CustomerInvoicePayDto customerInvoicePay)
        {
            _ = await _context.CustomerInvoicePays.FirstOrDefaultAsync(p => p.CustomerInvoicePayId == customerInvoicePay.CustomerInvoicePayId)
                ?? throw new KeyNotFoundException("Customer Invoice Pay not found");

            var invoicePay = CustomerInvoiceMapper.ToModel(customerInvoicePay);
            _context.CustomerInvoicePays.Update(invoicePay);

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsDeleteCustomerInvoicePayAsync(Guid id)
        {
            var customerInvoicePay = await _context.CustomerInvoicePays.FirstOrDefaultAsync(p => p.CustomerInvoicePayId == id)
                ?? throw new KeyNotFoundException("Customer Invoice Pay not found");

            customerInvoicePay.Status = GeneralStatuses.Deleted;
            _context.CustomerInvoicePays.Update(customerInvoicePay);

            await _context.SaveChangesAsync();
        }
    }
}
