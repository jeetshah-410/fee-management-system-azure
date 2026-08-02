using System.Threading.Tasks;
using FeeManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.IO;

namespace FeeManagement.Functions.Endpoints;

public class AdminFeeFunction
{
    private readonly IStudentRepository _repository;
    private readonly ILogger<AdminFeeFunction> _logger;

    public AdminFeeFunction(IStudentRepository repository, ILogger<AdminFeeFunction> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Function("GetStudentDetailsAdmin")]
    public async Task<IActionResult> GetStudentDetails(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "management/students/{studentId:int}")] HttpRequest req,
        int studentId)
    {
        _logger.LogInformation("Admin fetching details for student {StudentId}", studentId);

        var authResult = await req.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authResult.Succeeded)
        {
            return new UnauthorizedObjectResult(new { Message = "Unauthorized: Invalid or missing Bearer token.", Error = authResult.Failure?.Message });
        }

        if (!authResult.Principal.IsInRole("Fee.Admin"))
        {
            return new ObjectResult(new { Message = "Forbidden: Insufficient privileges. Required role: Fee.Admin" }) 
            { 
                StatusCode = StatusCodes.Status403Forbidden 
            };
        }

        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            return new NotFoundObjectResult(new { Message = $"Student {studentId} not found." });
        }

        return new OkObjectResult(student);
    }

    [Function("UpdateFeeRecord")]
    public async Task<IActionResult> UpdateFee(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "management/students/{studentId:int}/fees")] HttpRequest req,
        int studentId)
    {
        _logger.LogInformation("Admin updating fees for student {StudentId}", studentId);

        var authResult = await req.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authResult.Succeeded)
        {
            return new UnauthorizedObjectResult(new { Message = "Unauthorized: Invalid or missing Bearer token.", Error = authResult.Failure?.Message });
        }

        if (!authResult.Principal.IsInRole("Fee.Admin"))
        {
            return new ObjectResult(new { Message = "Forbidden: Insufficient privileges. Required role: Fee.Admin" }) 
            { 
                StatusCode = StatusCodes.Status403Forbidden 
            };
        }

        var student = await _repository.GetByIdAsync(studentId);
        if (student == null)
        {
            return new NotFoundObjectResult(new { Message = $"Student {studentId} not found." });
        }

        using var reader = new StreamReader(req.Body);
        var body = await reader.ReadToEndAsync();
        var data = JsonSerializer.Deserialize<UpdateFeeRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data == null)
        {
            return new BadRequestObjectResult(new { Message = "Invalid request body." });
        }

        student.PaidAmount = data.PaidAmount;
        await _repository.UpdateAsync(student);

        return new OkObjectResult(new { Message = "Fee record updated successfully.", student.PaidAmount });
    }
}

public class UpdateFeeRequest
{
    public decimal PaidAmount { get; set; }
}
