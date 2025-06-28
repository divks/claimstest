using Claims.Auditing;
using Claims.Data;
using Claims.Repositories;
using Claims.Repositories.Interfaces;
using Claims.Services;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Database configuration - supports both Docker and local development
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB");
var mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "ClaimsDatabase";

if (string.IsNullOrEmpty(mongoConnectionString))
{
    // Use in-memory databases for local development (no Docker required)
    Console.WriteLine("🏠 Using in-memory databases for local development");
    
    builder.Services.AddDbContext<AuditContext>(options =>
        options.UseInMemoryDatabase("AuditDb"));

    builder.Services.AddDbContext<ClaimsContext>(options =>
        options.UseInMemoryDatabase("ClaimsDb"));
}
else
{
    // Use MongoDB for Docker/Production
    Console.WriteLine($"🍃 Using MongoDB: {mongoConnectionString.Replace("Claims123!", "***")}");
    
    // Use MongoDB for auditing too
    builder.Services.AddDbContext<AuditContext>(options =>
        options.UseInMemoryDatabase("AuditDb")); // Keep audit in-memory for simplicity

    builder.Services.AddDbContext<ClaimsContext>(options =>
    {
        var client = new MongoClient(mongoConnectionString);
        var database = client.GetDatabase(mongoDatabaseName);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    });
}

// Register repositories
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ICoverRepository, CoverRepository>();

// Register services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Claims API", Version = "v1" });
    // c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Claims.xml"), true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Initialize databases
using (var scope = app.Services.CreateScope())
{
    var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();
    
    // Always use in-memory for audit (or could be MongoDB too)
    Console.WriteLine("💾 Initializing audit database...");
    auditContext.Database.EnsureCreated();
}

app.Run();

public partial class Program { }
