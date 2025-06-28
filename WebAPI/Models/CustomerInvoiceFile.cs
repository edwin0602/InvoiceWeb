using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class CustomerInvoiceFile
    {
        [Key]
        public Guid CustomerInvoiceFileId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerInvoiceId { get; set; }

        [ForeignKey(nameof(CustomerInvoiceId))]
        [JsonIgnore]
        public virtual CustomerInvoice CustomerInvoice { get; set; }

        [Required]
        [StringLength(256)]
        public string FileName { get; set; }

        [Required]
        [StringLength(512)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(50)]
        public string FileType { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }
    }
}