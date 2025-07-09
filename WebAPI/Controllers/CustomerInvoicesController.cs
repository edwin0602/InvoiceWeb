using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WebAPI.Dtos;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerInvoicesController : ControllerBase
    {
        private readonly ICustomerInvoiceService _service;

        public CustomerInvoicesController(ICustomerInvoiceService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerInvoiceDto>> GetCustomerInvoice(Guid id)
        {
            try
            {
                var invoice = await _service.GetCustomerInvoiceByIdAsync(id);
                if (invoice == null)
                {
                    return NotFound();
                }
                return Ok(invoice);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerInvoiceDto>>> GetAllCustomerInvoices()
        {
            var invoices = await _service.GetAllCustomerInvoicesAsync();
            if (invoices == null)
            {
                return NotFound();
            }
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomerInvoice([FromBody] CustomerInvoiceDto dto)
        {
            await _service.CreateCustomerInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetCustomerInvoice), new { id = dto.CustomerInvoiceId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] CustomerInvoiceDto updatedInvoice)
        {
            var updated = await _service.UpdateCustomerInvoiceAsync(id, updatedInvoice);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomerInvoice(Guid id)
        {
            try
            {
                await _service.DeleteCustomerInvoiceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("pdf/{id}")]
        public async Task<IActionResult> GenerateInvoicePdf(Guid id)
        {
            try
            {
                var pdfBytes = await _service.GenerateInvoicePdfAsync(id);
                return File(pdfBytes, "application/pdf", "invoice.pdf");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("email/{invoiceId}")]
        public async Task<IActionResult> SendInvoiceEmail(Guid invoiceId)
        {
            try
            {
                await _service.SendInvoiceEmailAsync(invoiceId);
                return Ok(new { message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("{invoiceId}/files")]
        public async Task<IActionResult> AddFileToInvoice(Guid invoiceId, [FromForm] IFormFile file, [FromForm] string fileType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            await _service.AddFileToInvoiceAsync(invoiceId, file, fileType);
            return Ok();
        }

        [HttpDelete("{invoiceId}/files/{fileId}")]
        public async Task<IActionResult> MarkCustomerFileAsDeleteAsync(Guid invoiceId, Guid fileId)
        {
            await _service.MarkInvoiceFileAsDeleteAsync(invoiceId, fileId);
            return NoContent();
        }

        [HttpGet("{invoiceId}/files/{invoiceFileId}/download")]
        public async Task<IActionResult> DownloadInvoiceFile(Guid invoiceId, Guid invoiceFileId)
        {
            var result = await _service.DownloadInvoiceFileAsync(invoiceId, invoiceFileId);
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