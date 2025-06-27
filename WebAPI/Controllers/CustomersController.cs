using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;
using WebAPI.Dtos;

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
        public async Task<IActionResult> AddFileToCustomer(Guid customerId, [FromBody] CustomerFileDto fileDto)
        {
            if (fileDto == null)
                return BadRequest();

            await _customerService.AddFileToCustomerAsync(customerId, fileDto);
            return Ok();
        }

        [HttpPut("{customerId}/files/{fileId}/status")]
        public async Task<IActionResult> UpdateCustomerFileStatus(Guid customerId, Guid fileId, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return BadRequest("Status is required.");

            await _customerService.UpdateCustomerFileStatusAsync(customerId, fileId, newStatus);
            return NoContent();
        }
    }
}
