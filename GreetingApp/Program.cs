using BusinessLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using NLog;
using NLog.Web;
using GreetingApp.Middleware;
using System.Text;
using BusinessLayer.Services;

var logger = LogManager.Setup().LoadConfigurationFromFile("Nlog.config").GetCurrentClassLogger();
logger.Info("Application is starting...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var jwtSettings = builder.Configuration.GetSection("Jwt");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });
    builder.Services.AddAuthorization();
    // Configure NLog
    builder.Logging.ClearProviders(); // Remove default logging providers
    builder.Host.UseNLog(); // Use NLog for logging

    // Add services to the container.
    builder.Services.AddScoped<IGreetingBL, GreetingBL>();
    builder.Services.AddScoped<IGreetingRL, GreetingRL>();
    builder.Services.AddScoped<TokenService>();

    builder.Services.AddControllers();
    builder.Services.AddDbContext<UserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    var app = builder.Build();

    app.UseGlobalExcepMiddleware();

    // Configure the HTTP request pipeline.
    app.UseHttpsRedirection();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();
    app.MapControllers();

    logger.Info("Application has started successfully.");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application startup failed!");
}
finally
{
    LogManager.Shutdown(); // Ensure all logs are written before application exits
}
