using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicePayController : ControllerBase
    {
        private readonly ICustomerInvoicePayService _service;
        public InvoicePayController(ICustomerInvoicePayService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerInvoicePayDto>> GetInvoicePayById(Guid id)
        {
            try
            {
                var invoicePay = await _service.GetCustomerInvoicePayByIdAsync(id);
                return Ok(invoicePay);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoicePay([FromBody] CustomerInvoicePayDto dto)
        {
            await _service.AddCustomerInvoicePayAsync(dto);
            return CreatedAtAction(nameof(GetInvoicePayById), new { id = dto.CustomerInvoicePayId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoicePay(Guid id, [FromBody] CustomerInvoicePayDto updatedDto)
        {
            updatedDto.CustomerInvoicePayId = id;
            await _service.UpdateCustomerInvoicePayAsync(updatedDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoicePay(Guid id)
        {
            await _service.MarkAsDeleteCustomerInvoicePayAsync(id);
            return NoContent();
        }
    }
}
