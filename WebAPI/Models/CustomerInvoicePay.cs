using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class CustomerInvoicePay
    {
        [Key]
        public Guid CustomerInvoicePayId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CustomerInvoiceId { get; set; }

        [ForeignKey(nameof(CustomerInvoiceId))]
        [JsonIgnore]
        public virtual CustomerInvoice CustomerInvoice { get; set; }

        public Guid User_id { get; set; }

        [ForeignKey(nameof(User_id))]
        [JsonIgnore]
        public virtual User User { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}