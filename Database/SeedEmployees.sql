USE [TestDataBase];
GO
IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
    THROW 50001, 'Employees table missing. Apply the EF Core InitialCreate migration first.', 1;
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = N'anita.kumar@example.com')
BEGIN
    INSERT dbo.Employees (Id, FirstName, LastName, Email, Department, DateOfJoining, IsActive, CreatedUtc)
    VALUES (NEWID(), N'Anita', N'Kumar', N'anita.kumar@example.com', N'Engineering', '2026-11-13', 1, SYSUTCDATETIME());
END;
IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = N'rahul.shah@example.com')
BEGIN
    INSERT dbo.Employees (Id, FirstName, LastName, Email, Department, DateOfJoining, IsActive, CreatedUtc)
    VALUES (NEWID(), N'Rahul', N'Shah', N'rahul.shah@example.com', N'Operations', '2025-06-01', 1, SYSUTCDATETIME());
END;
GO
SELECT Id, FirstName, LastName, Email, Department FROM dbo.Employees;
