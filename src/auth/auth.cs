using DevConnect.Auth;
using DevConnect.Data;

class AuthenticationService
{
    public List<User> Authenticate(string username, string password, AppDbContext db)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return new List<User> { };
        }

        if (username.Length < 3 || password.Length < 6)
        {
            return new List<User> { };
        }

        // create an instance of Users class and call the LoginAsync method
        Users user = new Users(db);

        var users = user.LoginAsync(username, password).Result;
        if (users.Count == 0)
        {
            return new List<User>();
        }

        return users?.Select(u => new User(u.Email, u.Name, u.Role, u.Level)).ToList() ?? new List<User>();
    }
}