using System;
using FeeManagement.Domain.Services;
using Xunit;

namespace FeeManagement.Tests.Domain;

public class FeeStatusCalculatorTests
{
    [Fact]
    public void Calculate_WhenPaidInFull_ReturnsPaid()
    {
        var status = FeeStatusCalculator.CalculateStatus(5000m, 5000m, DateTime.UtcNow.AddDays(10));
        Assert.Equal("Paid", status);
    }

    [Fact]
    public void Calculate_WhenOverpaid_ReturnsPaid()
    {
        var status = FeeStatusCalculator.CalculateStatus(5000m, 5500m, DateTime.UtcNow.AddDays(10));
        Assert.Equal("Paid", status);
    }

    [Fact]
    public void Calculate_WhenPartiallyPaidBeforeDueDate_ReturnsPartiallyPaid()
    {
        var status = FeeStatusCalculator.CalculateStatus(5000m, 2500m, DateTime.UtcNow.AddDays(10));
        Assert.Equal("Partially Paid", status);
    }

    [Fact]
    public void Calculate_WhenPartiallyPaidAndPastDue_ReturnsOverdue()
    {
        var status = FeeStatusCalculator.CalculateStatus(5000m, 2500m, DateTime.UtcNow.AddDays(-10));
        Assert.Equal("Overdue", status);
    }

    [Fact]
    public void Calculate_WhenUnpaidAndPastDue_ReturnsOverdue()
    {
        var status = FeeStatusCalculator.CalculateStatus(5000m, 0m, DateTime.UtcNow.AddDays(-10));
        Assert.Equal("Overdue", status);
    }

    [Fact]
    public void Calculate_WhenUnpaidBeforeDueDate_ReturnsPartiallyPaid()
    {
        // This is the edge case we discussed, returning Partially Paid for 0 amount before due date
        var status = FeeStatusCalculator.CalculateStatus(5000m, 0m, DateTime.UtcNow.AddDays(10));
        Assert.Equal("Partially Paid", status);
    }
}
