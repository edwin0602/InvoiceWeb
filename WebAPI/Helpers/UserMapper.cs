using WebAPI.Models;
using WebAPI.Dtos;

namespace WebAPI.Helpers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                EmailAddress = user.EmailAddress,
                PhotoUrl = user.PhotoUrl,
                PasswordHash = user.PasswordHash,
                Role_id = user.Role_id,
                RoleName = user.Role?.RoleName,
                CreationDate = user.CreationDate,
                UpdateDate = user.UpdateDate,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DocumentNumber = user.DocumentNumber,
                Status = user.Status,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
        }

        public static User ToModel(UserDto dto)
        {
            return new User
            {
                UserId = dto.UserId,
                Username = dto.Username,
                EmailAddress = dto.EmailAddress,
                PhotoUrl = dto.PhotoUrl,
                PasswordHash = dto.PasswordHash,
                Role_id = dto.Role_id,
                CreationDate = dto.CreationDate ?? DateTime.UtcNow,
                UpdateDate = dto.UpdateDate,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DocumentNumber = dto.DocumentNumber,
                Status = dto.Status,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber
            };
        }
    }
}