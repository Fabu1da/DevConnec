using DevConnect.Auth;
using DevConnect.Data;
using Microsoft.EntityFrameworkCore;

namespace DevConnect.Users
{ 
    class Users
    {
            private readonly AppDbContext _db;

            public Users(AppDbContext db)
            {
                _db = db;
            }

             public async Task<List<User>> GetAllUsersAsync(AppDbContext db)
            {
                return await db.Users.Select(u => new User(u.Email, u.Name, u.Role, u.Level)).ToListAsync();
            }
        }
    }