using System.ComponentModel.DataAnnotations;
using WebAPI.Common.Constants;

namespace WebAPI.Models
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(512)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(256)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = GeneralStatuses.Active;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<CustomerInvoice> CustomerInvoices { get; set; } = new List<CustomerInvoice>();

        public virtual ICollection<CustomerFile> CustomerFiles { get; set; } = new List<CustomerFile>();
    }
}
