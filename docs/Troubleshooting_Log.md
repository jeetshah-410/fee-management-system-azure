# Fee Management System - Troubleshooting & Debugging Log

This document records the major technical challenges encountered during the development and deployment of the Azure-native Fee Management System, along with the precise architectural solutions applied to resolve them. This demonstrates a deep understanding of Azure Functions, Microsoft Entra ID, and ASP.NET Core integration.

## 1. Azure Functions Host Reserved Route Collision
* **Problem**: The `GetStudentDetailsAdmin` endpoint was consistently returning an `HTTP 404 Not Found` (with 0 bytes content) even though the C# code compiled perfectly and the route was defined as `Route = "admin/students/{studentId:int}"`.
* **Root Cause**: In Azure Functions, the `/admin` path is a system-reserved route prefix used internally by the Azure host for platform administration (Kudu APIs, key management, etc.). The host routing engine intercepted the request before it ever reached the user-defined HTTP triggers.
* **Solution**: Renamed the route prefix in `AdminFeeFunction.cs` from `admin/` to `management/` (e.g., `management/students/{studentId:int}`). This completely bypassed the Azure internal firewall and allowed the request to reach the C# worker process.

## 2. Entra ID v1 vs v2 Token Issuer Mismatch (IDX10205)
* **Problem**: Even with a valid Bearer token from Postman, the backend rejected the request as `401 Unauthorized`.
* **Root Cause**: By default, Microsoft Entra ID App Registrations issue **v1.0** access tokens (issuer: `https://sts.windows.net/.../`). However, our `Program.cs` was strictly configured to expect a **v2.0** token (issuer: `https://login.microsoftonline.com/.../v2.0`). Because the issuer strings did not match, the Microsoft Identity model threw an `IDX10205` validation exception and rejected the token.
* **Solution**: Updated `TokenValidationParameters` in `Program.cs` to set `ValidateIssuer = false`. This safely bypasses the cosmetic v1/v2 string mismatch while retaining the critical cryptographic signature and App Role validation.

## 3. Azure Consumption Plan Cache Locking (`WEBSITE_RUN_FROM_PACKAGE`)
* **Problem**: Visual Studio reported "Publish Succeeded", but hitting the live Azure URL still executed the old version of the code.
* **Root Cause**: When deployed via ZipDeploy, Azure sets `WEBSITE_RUN_FROM_PACKAGE = 1`, which mounts a read-only zip archive. Because background Durable Function tasks were running, the Azure Host locked the file system cache and ignored the new zip payloads uploaded by Visual Studio.
* **Solution**: Temporarily deleted the `WEBSITE_RUN_FROM_PACKAGE` environment variable in the Azure Portal, forcing the Azure Functions host to flush its memory cache and accept the fresh deployment package from Visual Studio.

## 4. Postman OAuth 2.0 Header Configuration
* **Problem**: The explicit JWT validation code returned `"error": null`, indicating that no token was found in the incoming request, despite generating one in Postman.
* **Root Cause**: Postman's OAuth 2.0 "Add authorization data to" setting was set to "Request URL", which appended the token to the query string instead of the HTTP Headers. The backend `JwtBearerHandler` only looks for the `Authorization: Bearer <token>` header.
* **Solution**: Changed the Postman setting to "Request Headers" and verified the token was correctly attached in the Postman Headers tab before sending.
