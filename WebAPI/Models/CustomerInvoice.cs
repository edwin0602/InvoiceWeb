using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebAPI.Models;
using System.Text.Json.Serialization;
using WebAPI.Common.Constants;

public class CustomerInvoice
{
    [Key]
    public Guid CustomerInvoiceId { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Status { get; set; } = InvoiceStatuses.Created;

    [Required]
    public string Consecutive { get; set; }

    [Required]
    [StringLength(50)]
    public string InvoiceType { get; set; } = InvoiceTypes.Draft;

    [Required]
    public Guid Customer_id { get; set; }

    [ForeignKey(nameof(Customer_id))]
    [JsonIgnore]
    public virtual Customer Customer { get; set; }

    public Guid User_id { get; set; }

    [ForeignKey(nameof(User_id))]
    [JsonIgnore]
    public virtual User User { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotalAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal VatAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public virtual ICollection<CustomerInvoiceLine> CustomerInvoiceLines { get; set; } = new List<CustomerInvoiceLine>();

    public virtual ICollection<CustomerInvoiceFile> CustomerInvoiceFiles { get; set; } = new List<CustomerInvoiceFile>();

    public virtual ICollection<CustomerInvoiceNote> CustomerInvoiceNotes { get; set; } = new List<CustomerInvoiceNote>();

    public virtual ICollection<CustomerInvoicePay> CustomerInvoicePays { get; set; } = new List<CustomerInvoicePay>();

    [Required]
    public Guid Vat_id { get; set; }

    [ForeignKey(nameof(Vat_id))]
    [JsonIgnore]
    public virtual VAT VAT { get; set; }

}
