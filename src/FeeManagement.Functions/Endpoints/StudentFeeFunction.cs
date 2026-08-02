using System.Threading.Tasks;
using FeeManagement.Domain.Interfaces;
using FeeManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FeeManagement.Functions.Endpoints;

public class StudentFeeFunction
{
    private readonly IStudentRepository _repository;
    private readonly ILogger<StudentFeeFunction> _logger;

    public StudentFeeFunction(IStudentRepository repository, ILogger<StudentFeeFunction> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Function("GetStudentFeeStatus")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "students/{studentId:int}/fees")] HttpRequest req,
        int studentId)
    {
        _logger.LogInformation("Fetching fee status for student {StudentId}", studentId);

        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            return new NotFoundObjectResult(new { Message = $"Student {studentId} not found." });
        }

        var status = FeeStatusCalculator.CalculateStatus(student.TotalFee, student.PaidAmount, student.DueDate);

        return new OkObjectResult(new
        {
            student.StudentID,
            student.Name,
            student.Course,
            student.TotalFee,
            student.PaidAmount,
            student.DueDate,
            PaymentStatus = status
        });
    }
}
