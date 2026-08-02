using System;
using System.Linq;
using System.Threading.Tasks;
using FeeManagement.Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FeeManagement.Functions.Orchestrations;

public class ReminderOrchestrator
{
    [Function("TimerStart_ReminderOrchestrator")]
    public static async Task TimerStart(
        [TimerTrigger("0 0 8 * * *")] TimerInfo timer,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("TimerStart_ReminderOrchestrator");
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("ReminderOrchestrator");
        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
    }

    [Function("ReminderOrchestrator")]
    public static async Task RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger("ReminderOrchestrator");

        logger.LogInformation("Fetching overdue students.");
        var overdueStudents = await context.CallActivityAsync<Student[]>("GetOverdueStudentsActivity", "");

        if (overdueStudents == null || overdueStudents.Length == 0)
        {
            logger.LogInformation("No overdue students found today.");
            return;
        }

        logger.LogInformation("Fanning out reminders to {Count} students.", overdueStudents.Length);

        // Native Retry configuration for the activity
        var retryPolicy = new RetryPolicy(
            maxNumberOfAttempts: 3,
            firstRetryInterval: TimeSpan.FromSeconds(5),
            backoffCoefficient: 2.0);

        var taskOptions = TaskOptions.FromRetryPolicy(retryPolicy);

        var tasks = overdueStudents.Select(student =>
            context.CallActivityAsync("SendReminderActivity", student, taskOptions));

        await Task.WhenAll(tasks);
        
        logger.LogInformation("All reminders sent successfully.");
    }
}
