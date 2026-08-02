using System;

namespace FeeManagement.Domain.Entities;

public class Student
{
    public int StudentID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    
    // Fee-specific properties directly on the Student (per assignment schema)
    public decimal TotalFee { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime DueDate { get; set; }
    
    // Additional properties for notifications
    public string Email { get; set; } = string.Empty;
    public DateTime? LastReminderSentDate { get; set; }
}
