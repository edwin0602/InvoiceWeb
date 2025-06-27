namespace WebAPI.Dtos
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public ICollection<CustomerFileDto> CustomerFiles { get; set; } = new List<CustomerFileDto>();
    }
}