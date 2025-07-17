namespace WebAPI.Dtos
{
    public class CustomerInvoiceDto
    {
        public Guid CustomerInvoiceId { get; set; }
        public string? Status { get; set; }
        public string InvoiceType { get; set; }
        public string Consecutive { get; set; }
        public Guid Customer_id { get; set; }
        public Guid User_id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<CustomerInvoiceLineDto> CustomerInvoiceLines { get; set; } = new List<CustomerInvoiceLineDto>();
        public ICollection<CustomerInvoiceFileDto> CustomerInvoiceFiles { get; set; } = new List<CustomerInvoiceFileDto>();
        public ICollection<CustomerInvoiceNoteDto> CustomerInvoiceNotes { get; set; } = new List<CustomerInvoiceNoteDto>();
        public ICollection<CustomerInvoicePayDto> CustomerInvoicePays { get; set; } = new List<CustomerInvoicePayDto>();
        public Guid Vat_id { get; set; }
    }

    public class CustomerInvoiceLineDto
    {
        public Guid InvoiceLineId { get; set; }
        public Guid CustomerInvoice_id { get; set; }
        public Guid Item_id { get; set; }

        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
