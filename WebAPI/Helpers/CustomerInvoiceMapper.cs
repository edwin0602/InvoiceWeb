using WebAPI.Models;
using WebAPI.Dtos;

namespace WebAPI.Helpers
{
    public static class CustomerInvoiceMapper
    {
        public static CustomerInvoiceNoteDto ToDto(CustomerInvoiceNote note)
        {
            return new CustomerInvoiceNoteDto
            {
                CustomerInvoiceNoteId = note.CustomerInvoiceNoteId,
                CustomerInvoiceId = note.CustomerInvoiceId,
                Text = note.Text,
                Date = note.Date,
                Status = note.Status
            };
        }

        public static CustomerInvoiceNote ToModel(CustomerInvoiceNoteDto dto)
        {
            return new CustomerInvoiceNote
            {
                CustomerInvoiceNoteId = dto.CustomerInvoiceNoteId,
                CustomerInvoiceId = dto.CustomerInvoiceId,
                Text = dto.Text,
                Date = dto.Date,
                Status = dto.Status
            };
        }

        public static CustomerInvoiceFileDto ToDto(CustomerInvoiceFile file)
        {
            return new CustomerInvoiceFileDto
            {
                CustomerInvoiceFileId = file.CustomerInvoiceFileId,
                FileName = file.FileName,
                FilePath = file.FilePath,
                FileType = file.FileType,
                Status = file.Status,
                CreationDate = file.CreationDate,
                UpdateDate = file.UpdateDate
            };
        }

        public static CustomerInvoiceFile ToModel(CustomerInvoiceFileDto dto, Guid invoiceId)
        {
            return new CustomerInvoiceFile
            {
                CustomerInvoiceFileId = dto.CustomerInvoiceFileId != Guid.Empty ? dto.CustomerInvoiceFileId : Guid.NewGuid(),
                CustomerInvoiceId = invoiceId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                FileType = dto.FileType,
                Status = dto.Status,
                CreationDate = dto.CreationDate,
                UpdateDate = dto.UpdateDate
            };
        }

        public static CustomerInvoiceLineDto ToDto(CustomerInvoiceLine line)
        {
            return new CustomerInvoiceLineDto
            {
                InvoiceLineId = line.InvoiceLineId,
                CustomerInvoice_id = line.CustomerInvoice_id,
                Item_id = line.Item_id,
                ItemName = line.Item?.Name,
                ItemDescription = line.Item?.Description,
                Quantity = line.Quantity,
                Price = line.Price
            };
        }

        public static CustomerInvoiceLine ToModel(CustomerInvoiceLineDto dto)
        {
            return new CustomerInvoiceLine
            {
                InvoiceLineId = dto.InvoiceLineId,
                CustomerInvoice_id = dto.CustomerInvoice_id,
                Item_id = dto.Item_id,
                Quantity = dto.Quantity,
                Price = dto.Price
            };
        }

        public static CustomerInvoicePayDto ToDto(CustomerInvoicePay pay)
        {
            return new CustomerInvoicePayDto
            {
                CustomerInvoicePayId = pay.CustomerInvoicePayId,
                CustomerInvoiceId = pay.CustomerInvoiceId,
                Description = pay.Description,
                Amount = pay.Amount,
                PaymentDate = pay.PaymentDate,
                PaymentMethod = pay.PaymentMethod,
                Status = pay.Status,
                UserName = pay.User?.Username
            };
        }

        public static CustomerInvoicePay ToModel(CustomerInvoicePayDto dto)
        {
            return new CustomerInvoicePay
            {
                CustomerInvoicePayId = dto.CustomerInvoicePayId != Guid.Empty ? dto.CustomerInvoicePayId : Guid.NewGuid(),
                CustomerInvoiceId = dto.CustomerInvoiceId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Description = dto.Description,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status
            };
        }

        public static CustomerInvoiceDto ToDto(CustomerInvoice invoice)
        {
            return new CustomerInvoiceDto
            {
                CustomerInvoiceId = invoice.CustomerInvoiceId,
                Status = invoice.Status,
                InvoiceType = invoice.InvoiceType,
                Consecutive = invoice.Consecutive,
                Customer_id = invoice.Customer_id,
                User_id = invoice.User_id,
                InvoiceDate = invoice.InvoiceDate,
                ExpirationDate = invoice.ExpirationDate,
                CreationDate = invoice.CreationDate,
                UpdateDate = invoice.UpdateDate,
                SubTotalAmount = invoice.SubTotalAmount,
                VatAmount = invoice.VatAmount,
                TotalAmount = invoice.TotalAmount,
                Vat_id = invoice.Vat_id,
                CustomerInvoiceLines = invoice.CustomerInvoiceLines?.Select(ToDto).ToList() ?? new List<CustomerInvoiceLineDto>(),
                CustomerInvoiceFiles = invoice.CustomerInvoiceFiles?.Select(ToDto).ToList() ?? new List<CustomerInvoiceFileDto>(),
                CustomerInvoiceNotes = invoice.CustomerInvoiceNotes?.Select(ToDto).ToList() ?? new List<CustomerInvoiceNoteDto>(),
                CustomerInvoicePays = invoice.CustomerInvoicePays?.Select(ToDto).ToList() ?? new List<CustomerInvoicePayDto>()
            };
        }

        public static CustomerInvoice ToModel(CustomerInvoiceDto dto)
        {
            return new CustomerInvoice
            {
                CustomerInvoiceId = dto.CustomerInvoiceId,
                Status = dto.Status,
                InvoiceType = dto.InvoiceType,
                Customer_id = dto.Customer_id,
                User_id = dto.User_id,
                InvoiceDate = dto.InvoiceDate,
                ExpirationDate = dto.ExpirationDate,
                CreationDate = dto.CreationDate,
                UpdateDate = dto.UpdateDate,
                SubTotalAmount = dto.SubTotalAmount,
                VatAmount = dto.VatAmount,
                TotalAmount = dto.TotalAmount,
                Vat_id = dto.Vat_id,
                CustomerInvoiceLines = dto.CustomerInvoiceLines?.Select(ToModel).ToList() ?? new List<CustomerInvoiceLine>()
            };
        }
    }
}