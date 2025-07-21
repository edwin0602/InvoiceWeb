namespace WebAPI.Dtos
{
    public class InvoicePaymentDto
    {
        public Guid CustomerInvoicePayId { get; set; }
        public Guid UserId { get; set; }
        public Guid CustomerInvoiceId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? UserName { get; set; }
    }
}
