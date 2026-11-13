using AspireEmployeeStarter.Api.Data;
using AspireEmployeeStarter.Api.DTOs;
using AspireEmployeeStarter.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspireEmployeeStarter.Api.Endpoints;
public static class EmployeeEndpoints
{
    private static EmployeeDto Map(Employee x) => new(x.Id, x.FirstName, x.LastName, x.Email, x.Department, x.DateOfJoining, x.IsActive, x.CreatedUtc);
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/employees").WithTags("Employees");
        group.MapGet("/", async (ApplicationDbContext db, CancellationToken ct) =>
        {
            var items = await db.Employees.AsNoTracking().OrderBy(x => x.LastName).ToListAsync(ct);
            return Results.Ok(items.Select(Map));
        });
        group.MapGet("/{id:int}", async (int id, ApplicationDbContext db, CancellationToken ct) =>
        {
            var x = await db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
            return x is null ? Results.NotFound() : Results.Ok(Map(x));
        });
        group.MapPost("/", async (CreateEmployeeDto input, ApplicationDbContext db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.LastName) ||
                string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Department))
                return Results.BadRequest(new { error = "Name, email and department are required." });
            var email = input.Email.Trim().ToLowerInvariant();
            if (await db.Employees.AnyAsync(e => e.Email == email, ct)) return Results.Conflict(new { error = "Email already exists." });
            var x = new Employee { FirstName = input.FirstName.Trim(), LastName = input.LastName.Trim(), Email = email, Department = input.Department.Trim(), DateOfJoining = input.DateOfJoining };
            db.Employees.Add(x);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/employees/{x.Id}", Map(x));
        });
        group.MapPut("/{id:int}", async (int id, UpdateEmployeeDto input, ApplicationDbContext db, CancellationToken ct) =>
        {
            var x = await db.Employees.FindAsync([id], ct);
            if (x is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.LastName) ||
                string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Department))
                return Results.BadRequest(new { error = "Name, email and department are required." });
            var email = input.Email.Trim().ToLowerInvariant();
            if (await db.Employees.AnyAsync(e => e.Email == email && e.Id != id, ct)) return Results.Conflict(new { error = "Email already exists." });
            x.FirstName = input.FirstName.Trim(); x.LastName = input.LastName.Trim(); x.Email = email;
            x.Department = input.Department.Trim(); x.DateOfJoining = input.DateOfJoining; x.IsActive = input.IsActive;
            await db.SaveChangesAsync(ct);
            return Results.Ok(Map(x));
        });
        group.MapDelete("/{id:guid}", async (int id, ApplicationDbContext db, CancellationToken ct) =>
        {
            var x = await db.Employees.FindAsync([id], ct);
            if (x is null) return Results.NotFound();
            db.Employees.Remove(x);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
        return endpoints;
    }
}
