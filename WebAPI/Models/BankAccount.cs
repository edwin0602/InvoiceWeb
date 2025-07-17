using System.ComponentModel.DataAnnotations;
using WebAPI.Common.Constants;

namespace WebAPI.Models
{
    public class BankAccount
    {
        [Key]
        public Guid BankAccountId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = GeneralStatuses.Active;

    }
}
