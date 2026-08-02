using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FeeManagement.Domain.Interfaces;
using FeeManagement.Infrastructure.Data;
using FeeManagement.Infrastructure.Repositories;
using FeeManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Standard ASP.NET Core auth
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tenantId = Environment.GetEnvironmentVariable("AzureAd__TenantId");
                var clientId = Environment.GetEnvironmentVariable("AzureAd__ClientId");

                options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                options.Audience = $"api://{clientId}";
                
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false
                };
            });
        services.AddAuthorization();

        services.AddDbContext<FeeDbContext>(options =>
            options.UseSqlServer(
                Environment.GetEnvironmentVariable("SqlConnectionString") ?? "Server=(localdb)\\mssqllocaldb;Database=FeeManagement;Trusted_Connection=True;",
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                }));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<INotificationService, SendGridNotificationService>();
    })
    .Build();

host.Run();
