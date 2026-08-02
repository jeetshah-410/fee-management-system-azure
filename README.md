# Fee Management System

A cloud-native student fee management system built on Azure, using Clean Architecture and Serverless computing.

## Architecture Overview

The system uses a 3-layer Clean Architecture approach:
1. **Domain**: Contains entities, enums, interfaces, and pure business logic (Zero external dependencies).
2. **Infrastructure**: Contains EF Core DbContext, repository implementations, and external services (SendGrid).
3. **Functions**: The Azure Functions (Isolated Worker .NET 8) host that wires dependencies and provides HTTP/Timer triggers.

```mermaid
graph TD
    %% Define Styles
    classDef client fill:#e1f5fe,stroke:#03a9f4,stroke-width:2px;
    classDef azure fill:#0078d4,stroke:#005a9e,stroke-width:2px,color:white;
    classDef app fill:#e8eaf6,stroke:#3f51b5,stroke-width:2px;
    classDef db fill:#fff3e0,stroke:#ff9800,stroke-width:2px;

    Student[Student Client]:::client
    Admin[Admin Client]:::client
    
    APIM[Azure API Management]:::azure
    Entra[Azure AD / Entra ID]:::azure
    
    subgraph Function App (Isolated Worker)
        AuthMiddleware[ASP.NET Core Auth Middleware]
        StudentEndpoint[Student Fee API]
        AdminEndpoint[Admin Fee API]
        ReminderOrchestrator[Reminder Orchestrator]
        SendActivity[Send Reminder Activity]
    end
    
    SQL[(Azure SQL Database)]:::db
    SendGrid[SendGrid / ACS Email]:::azure

    %% Flow
    Student -- API Key --> APIM
    Admin -- Login --> Entra
    Entra -- JWT --> APIM
    
    APIM --> AuthMiddleware
    AuthMiddleware --> StudentEndpoint
    AuthMiddleware -- "Role: Fee.Admin" --> AdminEndpoint
    
    StudentEndpoint --> SQL
    AdminEndpoint --> SQL
    
    Timer((Daily Timer)) --> ReminderOrchestrator
    ReminderOrchestrator -- Fan Out --> SendActivity
    SendActivity --> SQL
    SendActivity --> SendGrid
```

## Features
- **Student API**: View fee payment status (Paid, Partially Paid, Overdue). Secured by API Key.
- **Admin API**: View and update student fee records. Secured by Azure AD RBAC (`Fee.Admin` role).
- **Automated Reminders**: Daily Durable Functions orchestration fetches overdue students and sends email reminders in parallel with native retries.

## Documentation
For a complete overview of the system, deployment instructions, and operational guidance, please refer to the following documents:

- **[Solution Architecture](docs/Solution_Architecture.md)**: Deep dive into architectural decisions, security models, trade-offs, and data flows.
- **[Deployment Guide](docs/deployment-guide.md)**: Step-by-step Azure portal provisioning and configuration instructions.
- **[Troubleshooting Log](docs/Troubleshooting_Log.md)**: A record of technical hurdles overcome during development (e.g., Entra ID mismatches, Azure Function routing).

---
## Architecture Diagram
![System Architecture](docs/SystemArchitecture.png)
