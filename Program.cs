using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using DevConnect.Data;
using DevConnect.Auth;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.MapGet("/hello", () =>
{
    return new { message = "Hello C# API" };
}).WithName("GetHello");



app.MapPost("/CreateUser", async (CreateUserRequest request, AppDbContext db) =>
{
    Users users = new Users(db);
    List<User> createdUsers = await users.createUserAsync(request.Email, request.Password, request.Name, request.Role, request.Level);
    
    return new { message = "User created successfully", users = createdUsers };
}).WithName("CreateUser");

app.MapPost("/login", async (LoginRequest request, AppDbContext db) =>
{
    string username = request.Email;
    string password = request.Password;
    Console.WriteLine($"Received login request: {username} / {password}");
    AuthenticationService authService = new AuthenticationService();
    List<User> isAuthenticated = authService.Authenticate(username, password, db);
    if (isAuthenticated.Count > 0)
    {
        return Results.Ok(new { message = "Login successful", users = isAuthenticated });
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithName("Login");


app.Run();

public record CreateUserRequest(string Email, string Password, string Name, string Role, string Level);

