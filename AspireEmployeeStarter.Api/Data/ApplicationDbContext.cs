using AspireEmployeeStarter.Api.Entities;
using Microsoft.EntityFrameworkCore;
namespace AspireEmployeeStarter.Api.Data;
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.DateOfJoining)
                  .HasColumnName("DateJoined")
                  .HasColumnType("date")
                  .IsRequired();
        });
    }
}
