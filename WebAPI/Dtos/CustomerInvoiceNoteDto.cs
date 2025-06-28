namespace WebAPI.Dtos
{
    public class CustomerInvoiceNoteDto
    {
        public Guid CustomerInvoiceNoteId { get; set; }
        public Guid CustomerInvoiceId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}