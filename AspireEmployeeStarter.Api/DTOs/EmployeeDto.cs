namespace AspireEmployeeStarter.Api.DTOs;
public sealed record EmployeeDto(int Id, string FirstName, string LastName, string Email, string Department, DateOnly DateOfJoining, bool IsActive, DateTime CreatedUtc);
public sealed record CreateEmployeeDto(string FirstName, string LastName, string Email, string Department, DateOnly DateOfJoining);
public sealed record UpdateEmployeeDto(string FirstName, string LastName, string Email, string Department, DateOnly DateOfJoining, bool IsActive);
