using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IInvoicePaymentService _service;
        public PaymentsController(IInvoicePaymentService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoicePaymentDto>> GetInvoicePaymentById(Guid id)
        {
            try
            {
                var invoicePay = await _service.GetInvoicePaymentByIdAsync(id);
                return Ok(invoicePay);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoicePayment([FromBody] InvoicePaymentDto dto)
        {
            await _service.AddInvoicePaymentAsync(dto);
            return CreatedAtAction(nameof(GetInvoicePaymentById), new { id = dto.CustomerInvoicePayId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoicePayment(Guid id, [FromBody] InvoicePaymentDto updatedDto)
        {
            updatedDto.CustomerInvoicePayId = id;
            await _service.UpdateInvoicePaymentAsync(updatedDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoicePayment(Guid id)
        {
            await _service.MarkAsDeleteInvoicePaymentAsync(id);
            return NoContent();
        }
    }
}
