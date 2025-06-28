using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Dtos;
using System.Text;
using System.Security.Cryptography;
using WebAPI.Helpers;

namespace WebAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task AddUserAsync(UserDto userDto);
        Task UpdateUserAsync(Guid id, UserDto userDto);
        Task DeleteUserAsync(Guid id);
        Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);
    }

    public class UserService : IUserService
    {
        private readonly InvoicikaDbContext _context;

        public UserService(InvoicikaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return users.Select(UserMapper.ToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user == null ? null : UserMapper.ToDto(user);
        }

        public async Task AddUserAsync(UserDto userDto)
        {
            if (await UsernameExistsAsync(userDto.Username))
            {
                throw new ArgumentException("Username already exists.");
            }

            var user = UserMapper.ToModel(userDto);

            // Hash password if provided
            if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != "null")
            {
                user.PasswordHash = HashPassword(user.PasswordHash);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Guid id, UserDto userDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (await UsernameExistsAsync(userDto.Username, id))
            {
                throw new ArgumentException("Username already exists.");
            }

            user.Username = userDto.Username;
            user.EmailAddress = userDto.EmailAddress;
            user.PhotoUrl = userDto.PhotoUrl;
            user.Role_id = userDto.Role_id;
            user.UpdateDate = DateTime.UtcNow;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.DocumentNumber = userDto.DocumentNumber;
            user.Status = userDto.Status;
            user.Address = userDto.Address;
            user.PhoneNumber = userDto.PhoneNumber;

            if (!string.IsNullOrEmpty(userDto.PasswordHash) && userDto.PasswordHash != "null")
            {
                user.PasswordHash = HashPassword(userDto.PasswordHash);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username && (!excludeUserId.HasValue || u.UserId != excludeUserId.Value));
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
