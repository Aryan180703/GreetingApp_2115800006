using NLog;
using NLog.Web;
using BusinessLayer.Service;
using BusinessLayer.Interface;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Info("Application is starting...");

var builder = WebApplication.CreateBuilder(args);

// Configure NLog as the logging provider
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

//Register the GreetingService in dependency injection
builder.Services.AddScoped<IGreetingBL, GreetingBL>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    logger.Info("Application is running...");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application failed to start.");
    throw;
}
finally
{
    LogManager.Shutdown();
}

app.Run();
