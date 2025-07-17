using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Data;
using WebAPI.Helpers;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using WebAPI.Common.Constants;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface ICustomerInvoiceService
    {
        Task<CustomerInvoiceDto> GetCustomerInvoiceByIdAsync(Guid id);
        Task<IEnumerable<CustomerInvoiceDto>> GetAllCustomerInvoicesAsync();
        Task CreateCustomerInvoiceAsync(CustomerInvoiceDto dto);
        Task<bool> UpdateCustomerInvoiceAsync(Guid id, CustomerInvoiceDto dto);
        Task DeleteCustomerInvoiceAsync(Guid id);
        Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId);
        Task SendInvoiceEmailAsync(Guid invoiceId);
        Task AddFileToInvoiceAsync(Guid invoiceId, IFormFile file, string fileType);
        Task MarkInvoiceFileAsDeleteAsync(Guid invoiceId, Guid fileId);
        Task<(byte[] FileBytes, string FileName)?> DownloadInvoiceFileAsync(Guid invoiceId, Guid invoiceFileId);
    }

    public class CustomerInvoiceService : ICustomerInvoiceService
    {
        private readonly InvoicikaDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public CustomerInvoiceService(
            InvoicikaDbContext context,
            IConfiguration configuration,
            IFileService fileService)
        {
            _context = context;
            _configuration = configuration;
            _fileService = fileService;
        }

        public async Task<CustomerInvoiceDto> GetCustomerInvoiceByIdAsync(Guid id)
        {
            var invoice = await _context.CustomerInvoices
                .Include(c => c.Customer)
                .Include(c => c.User)
                .Include(c => c.VAT)
                .Include(c => c.CustomerInvoiceLines).ThenInclude(l => l.Item)
                .Include(c => c.CustomerInvoiceFiles)
                .Include(c => c.CustomerInvoiceNotes)
                .Include(c => c.CustomerInvoicePays)
                .FirstOrDefaultAsync(c => c.CustomerInvoiceId == id);

            if (invoice == null)
                throw new KeyNotFoundException("Customer Invoice Pay not found");

            return CustomerInvoiceMapper.ToDto(invoice);
        }

        public async Task<IEnumerable<CustomerInvoiceDto>> GetAllCustomerInvoicesAsync()
        {
            var invoices = await _context.CustomerInvoices
                .Include(c => c.Customer)
                .Include(c => c.User)
                .Include(c => c.VAT)
                .Include(c => c.CustomerInvoiceLines).ThenInclude(l => l.Item)
                .Include(c => c.CustomerInvoiceFiles)
                .ToListAsync();

            return invoices.Select(CustomerInvoiceMapper.ToDto);
        }

        public async Task CreateCustomerInvoiceAsync(CustomerInvoiceDto dto)
        {
            var invoice = CustomerInvoiceMapper.ToModel(dto);
            invoice.Status = InvoiceStatuses.Created;

            _context.CustomerInvoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCustomerInvoiceAsync(Guid id, CustomerInvoiceDto updatedInvoice)
        {
            var existingInvoice = await _context.CustomerInvoices
                .Include(i => i.CustomerInvoiceLines)
                .Include(i => i.CustomerInvoiceFiles)
                .FirstOrDefaultAsync(i => i.CustomerInvoiceId == id);

            if (existingInvoice == null) return false;

            existingInvoice.Customer_id = updatedInvoice.Customer_id;
            existingInvoice.User_id = updatedInvoice.User_id;
            existingInvoice.InvoiceDate = updatedInvoice.InvoiceDate;
            existingInvoice.ExpirationDate = updatedInvoice.ExpirationDate;
            existingInvoice.UpdateDate = DateTime.UtcNow;
            existingInvoice.SubTotalAmount = updatedInvoice.SubTotalAmount;
            existingInvoice.VatAmount = updatedInvoice.VatAmount;
            existingInvoice.TotalAmount = updatedInvoice.TotalAmount;
            existingInvoice.Vat_id = updatedInvoice.Vat_id;
            existingInvoice.Status = updatedInvoice.Status;
            existingInvoice.InvoiceType = updatedInvoice.InvoiceType;

            existingInvoice.CustomerInvoiceLines.Clear();
            foreach (var lineDto in updatedInvoice.CustomerInvoiceLines)
            {
                existingInvoice.CustomerInvoiceLines.Add(CustomerInvoiceMapper.ToModel(lineDto));
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteCustomerInvoiceAsync(Guid id)
        {
            var invoice = await _context.CustomerInvoices.FindAsync(id);

            if (invoice == null) throw new KeyNotFoundException("Invoice not found");

            _context.CustomerInvoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task AddFileToInvoiceAsync(Guid invoiceId, IFormFile file, string fileType)
        {
            var invoice = await _context.CustomerInvoices
                .Include(i => i.CustomerInvoiceFiles)
                .FirstOrDefaultAsync(i => i.CustomerInvoiceId == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            var category = Path.Combine("invoices", invoiceId.ToString());
            var relativePath = await _fileService.SaveFileAsync(file, category);

            var invoiceFile = new CustomerInvoiceFile
            {
                CustomerInvoiceFileId = Guid.NewGuid(),
                Status =GeneralStatuses.Active,
                CustomerInvoiceId = invoiceId,
                FileName = file.FileName,
                FilePath = relativePath,
                FileType = fileType,
                CreationDate = DateTime.UtcNow
            };

            await _context.CustomerInvoiceFiles.AddAsync(invoiceFile);
            await _context.SaveChangesAsync();
        }

        public async Task MarkInvoiceFileAsDeleteAsync(Guid invoiceId, Guid fileId)
        {
            var invoice = await _context.CustomerInvoices
                .Include(i => i.CustomerInvoiceFiles)
                .FirstOrDefaultAsync(i => i.CustomerInvoiceId == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            var file = invoice.CustomerInvoiceFiles.FirstOrDefault(f => f.CustomerInvoiceFileId == fileId);
            if (file == null)
                throw new KeyNotFoundException("Invoice file not found");

            file.Status = GeneralStatuses.Deleted;
            file.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<(byte[] FileBytes, string FileName)?> DownloadInvoiceFileAsync(Guid invoiceId, Guid invoiceFileId)
        {
            var customerFile = await _context.CustomerInvoiceFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.CustomerInvoiceFileId == invoiceFileId);

            if (customerFile == null)
                return null;

            if (customerFile.CustomerInvoiceId != invoiceId)
                return null;

            var relativePath = customerFile.FilePath.StartsWith("/uploads/")
                ? customerFile.FilePath.Substring(9)
                : customerFile.FilePath;

            var fileBytes = await _fileService.GetFileAsync(relativePath);
            return (fileBytes, customerFile.FileName);
        }

        public async Task SendInvoiceEmailAsync(Guid invoiceId)
        {
            var invoiceDto = await GetCustomerInvoiceByIdAsync(invoiceId) ?? throw new Exception("Invoice not found.");
            var customer = await _context.Customers.FindAsync(invoiceDto.Customer_id) ?? throw new Exception("Customer not found.");
            var pdfBytes = await GenerateInvoicePdfAsync(invoiceId);

            // Get email configuration from app settings
            var smtpServer = _configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var senderEmail = _configuration["Email:SenderEmail"];
            var senderName = _configuration["Email:SenderName"];
            var senderPassword = _configuration["Email:SenderPassword"];

            // Create a new email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("Recipient", customer.Email));
            message.Subject = $"{customer.Name}, your Invoice from Invoicika";

            // Create the email body with both text and HTML
            var bodyBuilder = new BodyBuilder
            {
                TextBody = "Please find the attached invoice.",
                HtmlBody = $@"
            <p>Dear {customer.Name},</p>
            <p>Thank you for your recent transaction with Invoicika. Your invoice is attached to this email for your reference.</p>
            <p>Best regards,</p>
            <strong>Invoicika Team</strong>"
            };

            // Attach the PDF invoice
            using (var stream = new MemoryStream(pdfBytes))
            {
                bodyBuilder.Attachments.Add($"invoice-{invoiceId}.pdf", stream.ToArray(), new ContentType("application", "pdf"));
            }

            // Set the email body content
            message.Body = bodyBuilder.ToMessageBody();

            // Send the email via SMTP
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message}");
            }
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId)
        {
            var invoice = await _context.CustomerInvoices
                .Include(c => c.Customer)
                .Include(c => c.User)
                .Include(c => c.VAT)
                .Include(c => c.CustomerInvoiceLines)
                .ThenInclude(l => l.Item)
                .FirstOrDefaultAsync(c => c.CustomerInvoiceId == invoiceId);

            if (invoice == null)
                throw new ArgumentException("Invoice not found", nameof(invoiceId));

            return InvoicePdfGenerator.Generate(invoice);
        }
    }
}

