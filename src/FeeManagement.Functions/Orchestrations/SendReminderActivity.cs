using System;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;
using FeeManagement.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FeeManagement.Functions.Orchestrations;

public class SendReminderActivity
{
    private readonly IStudentRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SendReminderActivity> _logger;

    public SendReminderActivity(
        IStudentRepository repository, 
        INotificationService notificationService,
        ILogger<SendReminderActivity> logger)
    {
        _repository = repository;
        _notificationService = notificationService;
        _logger = logger;
    }

    [Function("GetOverdueStudentsActivity")]
    public async Task<Student[]> GetOverdueStudents([ActivityTrigger] string? input)
    {
        var students = await _repository.GetOverdueStudentsAsync();
        return students.ToArray();
    }

    [Function("SendReminderActivity")]
    public async Task SendReminder([ActivityTrigger] Student student)
    {
        _logger.LogInformation("Sending reminder to Student {StudentId}", student.StudentID);

        // Send the email (mocked in Infrastructure)
        await _notificationService.SendReminderAsync(student);

        // Update the last reminder sent date
        student.LastReminderSentDate = DateTime.UtcNow;
        await _repository.UpdateAsync(student);
    }
}
