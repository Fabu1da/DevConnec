using System.IdentityModel.Tokens.Jwt;
using DevConnect.Auth;
using DevConnect.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevConnect.Auth
{
    public record CreateUserRequest(string Email, string Password, string Name, string Role, string Level);
    public record User(string Email, string Name, string Role, string Level);
    public record LoginResult(string AccessToken, string RefreshToken, User User);

    public record CreateUserResponse(bool isCreated, string Message);


 


    class AuthenticationService
    {
        private readonly AppDbContext _db;
        public AuthenticationService(AppDbContext db)
        {
        _db = db;
        }
        public List<LoginResult> Authenticate(string username, string password, AppDbContext db)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new List<LoginResult> { };
            }

            if (username.Length < 3 || password.Length < 6)
            {
                return new List<LoginResult> { };
            }

            var users = LoginAsync(username, password).Result;

            return users == null ? new List<LoginResult> { } : new List<LoginResult> { users };
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

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password are required.");
            }
            
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            Console.WriteLine($"User found for email {email}: {user}");
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var hasher = new PasswordHasher<UserEntity>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.Password, password);
            Console.WriteLine($"Password verification result for {email}: {verificationResult}");
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid password.");
            }
            Console.WriteLine($"Password verification succeeded for {email}");
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(new JwtSecurityToken(
                claims: new[] { new System.Security.Claims.Claim("email", user.Email) },
                expires: DateTime.UtcNow.AddHours(1)
            ));
            var refreshToken = tokenHandler.WriteToken(new JwtSecurityToken(
                claims: new[] { new System.Security.Claims.Claim("email", user.Email) },
                expires: DateTime.UtcNow.AddDays(7)
            ));
            return new LoginResult(accessToken, refreshToken, new User(user.Email, user.Name, user.Role, user.Level));
        }

        public async Task<List<CreateUserResponse>> createUserAsync(string email, string password, string name, string role, string level)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(level))
            {
                return new List<CreateUserResponse>();
            }

            if (await IsExistAsync(email))
            {
                return new List<CreateUserResponse> { new CreateUserResponse(false, "User already exists") };
            }

            try
            {
            PasswordHasher<UserEntity> hasher = new PasswordHasher<UserEntity>();

            var user = new UserEntity { Email = email, Password = "", Name = name, Role = role, Level = level };
            var passwordHash = hasher.HashPassword(user, password);
            Console.WriteLine($"Password hash for {email}: {passwordHash}");
            await _db.Users.AddAsync(new UserEntity { Email = email, Name = name, Role = role, Level = level, Password = passwordHash });
            await _db.SaveChangesAsync();


            return new List<CreateUserResponse> { new CreateUserResponse(true, "User created successfully") };
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return new List<CreateUserResponse> { new CreateUserResponse(false, "Error creating user") };
            }
           
        }

    
    }
}