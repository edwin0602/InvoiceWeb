namespace WebAPI.Dtos
{
    public class CustomerInvoiceFileDto
    {
        public Guid CustomerInvoiceFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}