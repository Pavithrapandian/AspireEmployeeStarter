
using AspireEmployeeStarter.Api.Data;
using AspireEmployeeStarter.Api.Endpoints;
using AspireEmployeeStarter.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

// SQL Server LocalDB connection
var connectionString =
    builder.Configuration.GetConnectionString("sqldb")
    ?? @"Server=(localdb)\MSSQLLocalDB;Database=TestDataBase;Trusted_Connection=True;TrustServerCertificate=True;";

// Register EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Aspire health endpoints
app.MapDefaultEndpoints();

// Employee CRUD endpoints
app.MapEmployeeEndpoints();

// Database connection test
app.MapGet("/api/database/status",
    async (ApplicationDbContext db, CancellationToken ct) =>
    {
        var connected = await db.Database.CanConnectAsync(ct);

        return connected
            ? Results.Ok(new
            {
                database = db.Database.GetDbConnection().Database,
                status = "Connected"
            })
            : Results.Problem(
                "Unable to connect to SQL Server.",
                statusCode: 503);
    })
.WithTags("Database");

app.MapGet("/", () => Results.Ok(new
{
    application = "Aspire Employee Management API",
    status = "Running",
    database = "TestDataBase",
    endpoints = new[]
    {
        "/api/employees",
        "/api/database/status"
    }
}));

// Existing database: no automatic migrations
app.Run();

public partial class Program { }
