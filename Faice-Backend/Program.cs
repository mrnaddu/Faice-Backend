using Faice_Backend.Authorization;
using Faice_Backend.Data;
using Faice_Backend.Entities;
using Faice_Backend.Enums;
using Faice_Backend.Helpers;
using Faice_Backend.Interfaces;
using Faice_Backend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
var services = builder.Services;
var env = builder.Environment;


services.AddDbContext<FaiceDbContext>();

services.AddCors();

// Prevent json object from being self referenced multiple times when serialized to string
services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// configure strongly typed settings object
services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// configure DI for application services
services.AddScoped<IJwtUtilsService, JwtUtilsService>();
services.AddScoped<IAccountAppService, AccountAppService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Add authentication stuff
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();

app.UseAuthorization();

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

// create hardcoded test users in db on startup
{
    var testUsers = new List<User>
    {
        new() { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), Role = Role.Admin },
        new() { Id = 2, FirstName = "Normal", LastName = "User", Username = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user"), Role = Role.User }
    };

    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<FaiceDbContext>();
    dataContext.Users.AddRange(testUsers);
    dataContext.SaveChanges();
}

    app.Run();
