namespace WebAPI.Dtos
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PasswordHash { get; set; }
        public Guid Role_id { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Status { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
