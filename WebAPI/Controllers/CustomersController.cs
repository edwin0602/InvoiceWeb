using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WebAPI.Dtos;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _customerService.GetCustomersPagedAndSortedAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var customerDto = await _customerService.GetCustomerByIdAsync(customerId);
            if (customerDto == null)
            {
                return NotFound();
            }
            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest();
            }

            await _customerService.AddCustomerAsync(customerDto);
            return CreatedAtAction(nameof(GetCustomerById), new { customerId = customerDto.CustomerId }, customerDto);
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerDto customerDto)
        {
            if (customerId != customerDto.CustomerId)
            {
                return BadRequest("Customer ID mismatch");
            }

            var existingCustomer = await _customerService.GetCustomerByIdAsync(customerId);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            await _customerService.UpdateCustomerAsync(customerDto);
            return NoContent();
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            var existingCustomer = await _customerService.GetCustomerByIdAsync(customerId);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            await _customerService.DeleteCustomerAsync(customerId);
            return NoContent();
        }

        [HttpPost("{customerId}/files")]
        public async Task<IActionResult> AddFileToCustomer(Guid customerId, [FromForm] IFormFile file, [FromForm] string fileType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            await _customerService.AddFileToCustomerAsync(customerId, file, fileType);
            return Ok();
        }

        [HttpDelete("{customerId}/files/{fileId}")]
        public async Task<IActionResult> MarkCustomerFileAsDeleteAsync(Guid customerId, Guid fileId)
        {
            await _customerService.MarkCustomerFileAsDeleteAsync(customerId, fileId);
            return NoContent();
        }

        [HttpGet("{customerId}/files/{customerFileId}/download")]
        public async Task<IActionResult> DownloadCustomerFile(Guid customerId, Guid customerFileId)
        {
            var result = await _customerService.DownloadCustomerFileAsync(customerId, customerFileId);
            if (result == null)
                return NotFound("Archivo no encontrado.");

            if (!result.HasValue)
                return NotFound("Archivo no encontrado.");

            var (fileBytes, fileName) = result.Value;

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(fileName, out var contentType))
                contentType = "application/octet-stream";

            return File(fileBytes, contentType, fileName);
        }
    }
}
