using System;
using System.Linq;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;
using FeeManagement.Infrastructure.Data;
using FeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FeeManagement.Tests.Infrastructure;

public class StudentRepositoryTests
{
    private DbContextOptions<FeeDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<FeeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetOverdueStudentsAsync_ReturnsOnlyPastDueUnpaidStudents()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using (var context = new FeeDbContext(options))
        {
            context.Students.AddRange(
                new Student { StudentID = 1, TotalFee = 5000, PaidAmount = 5000, DueDate = DateTime.UtcNow.AddDays(-5) }, // Paid, past due
                new Student { StudentID = 2, TotalFee = 5000, PaidAmount = 2500, DueDate = DateTime.UtcNow.AddDays(5) },  // Partially paid, not due
                new Student { StudentID = 3, TotalFee = 5000, PaidAmount = 1000, DueDate = DateTime.UtcNow.AddDays(-5) }  // Partially paid, past due (OVERDUE)
            );
            await context.SaveChangesAsync();
        }

        using (var context = new FeeDbContext(options))
        {
            var repository = new StudentRepository(context);

            // Act
            var overdue = await repository.GetOverdueStudentsAsync();

            // Assert
            Assert.Single(overdue);
            Assert.Equal(3, overdue.First().StudentID);
        }
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        // Arrange
        var options = GetInMemoryOptions();
        using (var context = new FeeDbContext(options))
        {
            context.Students.Add(new Student { StudentID = 1, PaidAmount = 0 });
            await context.SaveChangesAsync();
        }

        using (var context = new FeeDbContext(options))
        {
            var repository = new StudentRepository(context);
            var student = await repository.GetByIdAsync(1);
            
            // Act
            student!.PaidAmount = 2000;
            await repository.UpdateAsync(student);
        }

        // Assert
        using (var context = new FeeDbContext(options))
        {
            var updatedStudent = await context.Students.FindAsync(1);
            Assert.Equal(2000, updatedStudent!.PaidAmount);
        }
    }
}
