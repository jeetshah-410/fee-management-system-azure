using System;

namespace FeeManagement.Domain.Services;

public static class FeeStatusCalculator
{
    public static string CalculateStatus(decimal totalFee, decimal paidAmount, DateTime dueDate)
    {
        if (paidAmount >= totalFee)
            return "Paid";

        if (DateTime.UtcNow > dueDate)
            return "Overdue";

        // Any unpaid amount before due date
        return "Partially Paid";
    }
}
