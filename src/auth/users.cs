using Microsoft.EntityFrameworkCore;
using DevConnect.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace DevConnect.Auth
{
    public record User(string Email, string Name, string Role, string Level);

    class Users
    {
        private readonly AppDbContext _db;

        public Users(AppDbContext db)
        {
            _db = db;
        }
        private async Task<bool> IsExistAsync(string email)
        {
            try
            {
                
                return await _db.Users.AnyAsync(u => u.Email == email);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new List<User>();
            }
            
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            Console.WriteLine($"User found for email", user);
            if (user == null)
            {
                return new List<User>();
            }

            // TODO: Implement proper password verification
            // For now, just return the user if found
            return await _db.Users.Where(u => u.Email == email).Select(u => new User(u.Email, u.Name ?? "", u.Role ?? "", u.Level ?? "")).ToListAsync();
        }

        public async Task<List<User>> createUserAsync(string email, string password, string name, string role, string level)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(level))
            {
                return new List<User>();
            }

            if (await IsExistAsync(email))
            {
                return new List<User>();
            }
            
            PasswordHasher<UserEntity> hasher = new PasswordHasher<UserEntity>();

            var user = new UserEntity { Email = email, Password = "", Name = name, Role = role, Level = level };
            var passwordHash = hasher.HashPassword(user, password);
            Console.WriteLine($"Password hash for {email}: {passwordHash}");
            await _db.Users.AddAsync(new UserEntity { Email = email, Name = name, Role = role, Level = level, Password = passwordHash });
            await _db.SaveChangesAsync();

            return await GetAllUsersAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _db.Users.Select(u => new User(u.Email, u.Name, u.Role, u.Level)).ToListAsync();
        }
    }
}
        