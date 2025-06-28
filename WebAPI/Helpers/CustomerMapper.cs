using WebAPI.Models;
using WebAPI.Dtos;
using WebAPI.Common.Constants;

namespace WebAPI.Helpers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(Customer customer)
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
                    .Select(ToDto)
                    .ToList() ?? new List<CustomerFileDto>()
            };
        }

        public static CustomerFileDto ToDto(CustomerFile file)
        {
            return new CustomerFileDto
            {
                CustomerFileId = file.CustomerFileId,
                FileName = file.FileName,
                FilePath = file.FilePath,
                FileType = file.FileType,
                Status = file.Status,
                CreationDate = file.CreationDate,
                UpdateDate = file.UpdateDate
            };
        }

        public static Customer ToModel(CustomerDto dto)
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