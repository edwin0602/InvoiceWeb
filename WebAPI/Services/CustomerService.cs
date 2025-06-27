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
        Task AddFileToCustomerAsync(Guid customerId, CustomerFileDto fileDto);
        Task UpdateCustomerFileStatusAsync(Guid customerId, Guid customerFileId, string newStatus);
    }

    public class CustomerService : ICustomerService
    {
        private readonly InvoicikaDbContext _context;

        public CustomerService(InvoicikaDbContext context)
        {
            _context = context;
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

            var customerDtos = customers.Select(MapToCustomerDto);

            return new PaginationResult<CustomerDto>(customerDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(i => i.CustomerId == id);

            return customer == null ? null : MapToCustomerDto(customer);
        }

        public async Task AddCustomerAsync(CustomerDto customerDto)
        {
            var customer = MapToCustomer(customerDto);
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

        public async Task AddFileToCustomerAsync(Guid customerId, CustomerFileDto fileDto)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerFiles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            var customerFile = new CustomerFile
            {
                CustomerFileId = Guid.NewGuid(),
                Status = GeneralStatuses.Active,
                CustomerId = customerId,
                FileName = fileDto.FileName,
                FilePath = fileDto.FilePath,
                FileType = fileDto.FileType,
                CreationDate = fileDto.CreationDate != default ? fileDto.CreationDate : DateTime.UtcNow,
                UpdateDate = fileDto.UpdateDate
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

        private CustomerDto MapToCustomerDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Status = customer.Status,
                CreationDate = customer.CreationDate,
                UpdateDate = customer.UpdateDate,
                CustomerFiles = customer.CustomerFiles?
                    .Select(f => new CustomerFileDto
                    {
                        CustomerFileId = f.CustomerFileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileType = f.FileType,
                        Status = f.Status,
                        CreationDate = f.CreationDate,
                        UpdateDate = f.UpdateDate
                    }).ToList() ?? new List<CustomerFileDto>()
            };
        }

        private Customer MapToCustomer(CustomerDto dto)
        {
            return new Customer
            {
                CustomerId = dto.CustomerId,
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Status = dto.Status ?? GeneralStatuses.Active,
                CreationDate = dto.CreationDate,
                UpdateDate = dto.UpdateDate
            };
        }
    }
}


