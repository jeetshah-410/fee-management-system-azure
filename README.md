# Fee Management System

A cloud-native student fee management system built on Azure, using Clean Architecture and Serverless computing.

## Architecture Overview

The system uses a 3-layer Clean Architecture approach:
1. **Domain**: Contains entities, enums, interfaces, and pure business logic (Zero external dependencies).
2. **Infrastructure**: Contains EF Core DbContext, repository implementations, and external services (SendGrid).
3. **Functions**: The Azure Functions (Isolated Worker .NET 8) host that wires dependencies and provides HTTP/Timer triggers.

## Features
- **Student API**: View fee payment status (Paid, Partially Paid, Overdue). Secured by API Key.
- **Admin API**: View and update student fee records. Secured by Azure AD RBAC (`Fee.Admin` role).
- **Automated Reminders**: Daily Durable Functions orchestration fetches overdue students and sends email reminders in parallel with native retries.

## Traceability (Requirements to Solution)
| Assignment Requirement | Implemented Solution |
|---|---|
| **Azure SQL Database** | Azure SQL + Entity Framework Core repository |
| **Fee calculations** | `FeeStatusCalculator` (Domain logic) |
| **Student API** | Azure Function + Azure API Management (API key rate limiting) |
| **Admin API** | Azure Function + Microsoft Entra ID RBAC (`Fee.Admin` App Role) |
| **Notifications** | Azure Durable Functions + SendGrid SMTP Relay |
| **API security** | Split Architecture: API keys for Students, Azure AD for Admins |
| **Monitoring** | Azure Application Insights (Live Metrics, App Map, KQL Logs) |
| **Retry policy** | Native Durable Task retry (`TaskOptions.FromRetryPolicy`) in notification activity |
| **5,000 students** | Indexed `StudentID` + Serverless Functions Fan-Out/Fan-In pattern |

## Documentation
For a complete overview of the system, deployment instructions, and operational guidance, please refer to the following documents:

- **[Solution Architecture](docs/Solution_Architecture.md)**: Deep dive into architectural decisions, security models, trade-offs, and data flows.
- **[Deployment Guide](docs/deployment-guide.md)**: Step-by-step Azure portal provisioning and configuration instructions.
- **[Troubleshooting Log](docs/Troubleshooting_Log.md)**: A record of technical hurdles overcome during development (e.g., Entra ID mismatches, Azure Function routing).

---
## Architecture Diagram
![System Architecture](docs/SystemArchitecture.png)
