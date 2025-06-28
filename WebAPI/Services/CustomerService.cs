using Microsoft.EntityFrameworkCore;
using WebAPI.Common.Constants;
using WebAPI.Data;
using WebAPI.Dtos;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface ICustomerService
    {
        Task<PaginationResult<CustomerDto>> GetCustomersPagedAndSortedAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
        Task AddCustomerAsync(CustomerDto customerDto);
        Task UpdateCustomerAsync(CustomerDto customerDto);
        Task DeleteCustomerAsync(Guid id);
        Task AddFileToCustomerAsync(Guid customerId, IFormFile file, string fileType);
        Task UpdateCustomerFileStatusAsync(Guid customerId, Guid customerFileId, string newStatus);
    }

    public class CustomerService : ICustomerService
    {
        private readonly InvoicikaDbContext _context;
        private readonly IFileService _fileService;

        public CustomerService(InvoicikaDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<PaginationResult<CustomerDto>> GetCustomersPagedAndSortedAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Customers
                .Include(c => c.CustomerFiles)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm) || (c.Email != null && c.Email.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderByDescending(i => i.CreationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerDtos = customers.Select(CustomerMapper.ToDto);

            return new PaginationResult<CustomerDto>(customerDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(i => i.CustomerId == id);

            return customer == null ? null : CustomerMapper.ToDto(customer);
        }

        public async Task AddCustomerAsync(CustomerDto customerDto)
        {
            var customer = CustomerMapper.ToModel(customerDto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(CustomerDto customerDto)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerDto.CustomerId);

            if (customer == null)
                return;

            customer.Name = customerDto.Name;
            customer.Address = customerDto.Address;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.Email = customerDto.Email;
            customer.Status = customerDto.Status;
            customer.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFileToCustomerAsync(Guid customerId, IFormFile file, string fileType)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            var category = Path.Combine("customers", customerId.ToString());
            var relativePath = await _fileService.SaveFileAsync(file, category);

            var customerFile = new CustomerFile
            {
                CustomerFileId = Guid.NewGuid(),
                Status = GeneralStatuses.Active,
                CustomerId = customerId,
                FileName = file.FileName,
                FilePath = relativePath,
                FileType = fileType,
                CreationDate = DateTime.UtcNow
            };

            customer.CustomerFiles.Add(customerFile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerFileStatusAsync(Guid customerId, Guid customerFileId, string newStatus)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            var customerFile = customer.CustomerFiles.FirstOrDefault(f => f.CustomerFileId == customerFileId);

            if (customerFile == null)
                throw new KeyNotFoundException("Customer file not found");

            customerFile.Status = newStatus;
            customerFile.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}


