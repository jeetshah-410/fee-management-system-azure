# Deployment Guide

This guide walks through deploying the Fee Management System to Azure using the portal and Azure CLI.

## Prerequisites
- Azure CLI installed
- .NET 8 SDK
- Azure Subscription

## 1. Resource Group
Create a resource group for all project resources.
```bash
az group create --name rg-feemanagement-prod --location eastus
```

## 2. Azure SQL Database (Free Tier)
We are using the Azure Portal to ensure the free tier is successfully applied.
1. Go to [portal.azure.com](https://portal.azure.com) and search for **Azure SQL**.
2. Click **+ Create** -> **SQL databases** (Create).
3. **CRITICAL**: Click the **Apply offer** button under the "Want to try Azure SQL Database for free?" banner to get the 100,000 vCore-seconds/month free tier.
4. Set Resource Group to `rg-feemanagement`.
5. Create a new Server with "SQL authentication" (or "Both"). Remember your login (e.g., `feeadmin`) and password!
6. On the **Networking** tab, set "Public endpoint" to **Yes**.
7. Check **"Allow Azure services and resources to access this server"** (so the Azure Function can connect).
8. Check **"Add current client IP address"** to **Yes** (so you can connect from your local machine to run scripts).
9. Click **Review + Create**.
10. Once deployed, get the Server name from the portal, connect via Azure Data Studio or SSMS, and execute `database/schema.sql` and `database/seed-data.sql`.

## 3. Azure Functions (Consumption Plan)
We are using the Azure Portal to ensure the serverless Consumption plan is selected.
1. In the Azure Portal search bar, type **Function App** and select it.
2. Click **+ Create** (or + Create Function App).
3. Under Hosting options, select **Consumption** (Serverless) and click Select.
4. Set Resource Group to `rg-feemanagement`.
5. **Function App name**: Pick a unique name (e.g., `func-feemanagement-yourname`).
6. **Runtime stack**: Select **.NET**.
7. **Version**: Select **8 (LTS), Isolated worker model**.
8. **Region**: Select the exact same region as your database (e.g., Central India).
9. **Operating System**: Windows (default).
10. Click **Review + Create**, then click **Create**.
11. Once deployed, click **Go to resource**. In the left menu, expand **Settings** and click **Environment variables**.
12. Add a new App Setting named `SqlConnectionString`. The value should be your Azure SQL connection string (found in the Database overview -> Connection strings -> ADO.NET). Replace `{your_password}` with your actual password!

## 4. Microsoft Entra ID (Azure AD) Setup
To secure the Admin endpoints with Role-Based Access Control (RBAC):
1. In the Azure Portal search bar, type **Microsoft Entra ID** and click it.
2. On the left menu, click **App registrations** and then click **+ New registration**.
3. **Name**: `FeeManagementAPI`.
4. **Account types**: Single tenant (the default). Click **Register**.
5. Once created, copy the **Application (client) ID** and **Directory (tenant) ID** from the Overview page.
6. On the left menu, click **Expose an API**.
7. Next to "Application ID URI", click **Add** and hit **Save** to accept the default `api://...` format.
8. On the left menu, click **App roles** and then click **+ Create app role**:
   - **Display name**: `Fee Admin`
   - **Allowed member types**: `Users/Groups`
   - **Value**: `Fee.Admin` (This is critical, it must match the C# code exactly)
   - **Description**: Allows admin to update fee records
   - Click **Apply**.
   *(Note: You must then assign this role to your user account under "Enterprise Applications" -> "Users and groups")*.
9. Go back to your **Function App -> Environment variables** and add two new variables:
   - `AzureAd__ClientId` = (Paste your Application client ID)
   - `AzureAd__TenantId` = (Paste your Directory tenant ID)

## 5. Azure API Management (Consumption Tier)
1. Create an APIM instance using the Consumption pricing tier.
2. Import the Function App into APIM.
3. For the Admin endpoints, leave the policy empty so APIM acts as a pure passthrough. The JWT token will be forwarded to the Azure Function where `AuthenticateAsync()` validates the signature and enforces the `Fee.Admin` role.
4. Enable "Subscription Required" on the Student endpoints to enforce API key rate-limiting.

## 6. Application Insights
1. Application Insights is created automatically with the Function App.
2. Navigate to the App Insights resource to view the Application Map, query `requests`, and trace Durable Function orchestration logs (`traces` where `operation_Name` = 'ReminderOrchestrator').
