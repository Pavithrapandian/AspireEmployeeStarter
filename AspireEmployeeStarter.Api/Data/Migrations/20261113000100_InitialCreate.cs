using System;
using AspireEmployeeStarter.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace AspireEmployeeStarter.Api.Data.Migrations;
[DbContext(typeof(ApplicationDbContext))]
[Migration("20261113000100_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("Employees", columns: table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            DateOfJoining = table.Column<DateOnly>(type: "date", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_Employees", x => x.Id));
        migrationBuilder.CreateIndex("IX_Employees_Email", "Employees", "Email", unique: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("Employees");
}
