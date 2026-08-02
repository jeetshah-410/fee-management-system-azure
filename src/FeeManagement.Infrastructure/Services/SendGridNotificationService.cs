using System;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;
using FeeManagement.Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FeeManagement.Infrastructure.Services;

public class SendGridNotificationService : INotificationService
{
    public async Task SendReminderAsync(Student student)
    {
        var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        var fromEmail = Environment.GetEnvironmentVariable("SendGridFromEmail");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fromEmail))
        {
            Console.WriteLine("[WARNING] SendGridApiKey or SendGridFromEmail is missing. Cannot send real email.");
            return;
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, "Fee Management System");
        var subject = "Action Required: Overdue Fee Reminder";
        var to = new EmailAddress(student.Email, student.Name);
        
        var plainTextContent = $"Dear {student.Name}, your fee balance of {student.TotalFee - student.PaidAmount:C} is overdue. Please log in to the portal to pay immediately.";
        var htmlContent = $"<strong>Dear {student.Name}</strong>,<br><br>Your fee balance of <strong style=\"color:red;\">{student.TotalFee - student.PaidAmount:C}</strong> is overdue. Please log in to the portal to pay immediately.";
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        
        var response = await client.SendEmailAsync(msg);
        
        Console.WriteLine($"[SENDGRID] Sent reminder to {student.Email}. SendGrid Status Code: {response.StatusCode}");
    }
}
