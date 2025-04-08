using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SeamlessDigital.ToDoSystem.Data;
using SeamlessDigital.ToDoSystem.Services.Implementations;
using SeamlessDigital.ToDoSystem.Services.Contracts;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using SeamlessDigital.ToDoSystem.Models;
using Serilog;
using Serilog.Exceptions;
using FluentValidation;
using SeamlessDigital.ToDoSystem.Validators;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// Add Serilog configuration to builder
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("Logs/ToDoLog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
        .ReadFrom.Configuration(context.Configuration));


// Add Serilog to the logging system
builder.Logging.ClearProviders();  // Clears the default logging providers
builder.Logging.AddSerilog();     // Add Serilog as the logging provider

// Load environment variables
Env.Load();
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
string? weatherApiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");

if (string.IsNullOrEmpty(connectionString))
{
    Log.Fatal("Database connection string not found.");
    throw new InvalidOperationException("Database connection string not found.");
}

if (string.IsNullOrEmpty(weatherApiKey))
{
    Log.Fatal("Weather API key not found.");
    throw new InvalidOperationException("Weather API key not found.");
}


builder.Services.AddValidatorsFromAssemblyContaining<TodoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// Add services to the container
builder.Services.AddDbContext<TodoContext>(options => options.UseSqlite(connectionString));

// Add HttpClient for DummyJsonAPIService (use DI)
builder.Services.AddHttpClient<DummyJsonAPIService>();

// Register WeatherAPIService with the API key from environment variable
builder.Services.AddSingleton<IWeatherAPIRepo>(new WeatherAPIService(weatherApiKey, new HttpClient()));

// Register TodoService (the actual service that handles CRUD operations for To-Do items)
builder.Services.AddScoped<ITodoRepo, TodoService>();

// Add controllers and configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Register Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Serilog for logging HTTP requests
app.UseSerilogRequestLogging(); // This method is part of Serilog.AspNetCore

// Apply database migrations and fetch to-do items
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
    dbContext.Database.Migrate();

    var todoService = scope.ServiceProvider.GetRequiredService<DummyJsonAPIService>();
    await todoService.FetchjsonTodosAsync();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
