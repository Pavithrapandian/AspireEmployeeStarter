
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Existing SQL Server LocalDB database
var connectionString =
    builder.Configuration.GetConnectionString("sqldb")
    ?? @"Server=(localdb)\MSSQLLocalDB;Database=TestDataBase;Trusted_Connection=True;TrustServerCertificate=True;";

// Register ASP.NET Core API
var api = builder
    .AddProject<Projects.AspireEmployeeStarter_Api>("api")
    .WithEnvironment("ConnectionStrings__sqldb", connectionString);

// React + Vite frontend
var webPath = Path.GetFullPath(
    Path.Combine(
        builder.AppHostDirectory,
        "..",
        "AspireEmployeeStarter.Web"
    )
);

// Use npm.cmd on Windows
var npmCommand = OperatingSystem.IsWindows()
    ? "npm.cmd"
    : "npm";

var web = builder
    .AddExecutable(
        "web",
        npmCommand,
        webPath,
        "run",
        "dev",
        "--",
        "--host",
        "127.0.0.1",
        "--port",
        "5173"
    )
    .WithHttpEndpoint(
        port: 5173,
        targetPort: 5173,
        isProxied: false
    )
    .WithEnvironment(
        "API_BASE_URL",
        api.GetEndpoint("http")
    )
    .WaitFor(api);

builder.Build().Run();
