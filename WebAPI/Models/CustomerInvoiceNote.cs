using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class CustomerInvoiceNote
    {
        [Key]
        public Guid CustomerInvoiceNoteId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerInvoiceId { get; set; }

        [ForeignKey(nameof(CustomerInvoiceId))]
        [JsonIgnore]
        public virtual CustomerInvoice CustomerInvoice { get; set; }

        [Required]
        [StringLength(1024)]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}