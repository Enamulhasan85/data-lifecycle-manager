using System.Text;
using DataLifecycleManager.Application.DTOs.SSISPackage;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Application.Services;

public class SSISCatalogService : ISSISCatalogService
{
    private readonly ILogger<SSISCatalogService> _logger;
    private readonly ISSISCatalogRepository _catalogRepository;

    public SSISCatalogService(
        ILogger<SSISCatalogService> logger,
        ISSISCatalogRepository catalogRepository)
    {
        _logger = logger;
        _catalogRepository = catalogRepository;
    }

    public async Task<PackageExistenceResultDto> CheckPackageExistsAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        string folderName,
        string projectName,
        string packageName)
    {
        try
        {
            var connectionString = useWindowsAuth
                ? $"Server={serverAddress};Database={catalogName};Integrated Security=true;TrustServerCertificate=true;"
                : $"Server={serverAddress};Database={catalogName};User ID={username};Password={password};TrustServerCertificate=true;";

            _logger.LogInformation("Checking package in {Catalog} on {Server}", catalogName, serverAddress);

            var folder = await _catalogRepository.GetFolderAsync(connectionString, catalogName, folderName);
            if (folder == null)
            {
                return new PackageExistenceResultDto
                {
                    Success = false,
                    ErrorMessage = $"Folder '{folderName}' not found in {catalogName} catalog.",
                    FolderExists = false,
                    ProjectExists = false,
                    PackageExists = false
                };
            }

            var project = await _catalogRepository.GetProjectAsync(connectionString, catalogName, folder.FolderId, projectName);
            if (project == null)
            {
                return new PackageExistenceResultDto
                {
                    Success = false,
                    ErrorMessage = $"Project '{projectName}' not found in folder '{folderName}'.",
                    FolderExists = true,
                    ProjectExists = false,
                    PackageExists = false
                };
            }

            var package = await _catalogRepository.GetPackageAsync(connectionString, catalogName, project.ProjectId, packageName);
            if (package == null)
            {
                return new PackageExistenceResultDto
                {
                    Success = false,
                    ErrorMessage = $"Package '{packageName}' not found in project '{projectName}'.",
                    FolderExists = true,
                    ProjectExists = true,
                    PackageExists = false
                };
            }

            var parameters = await _catalogRepository.GetProjectParametersAsync(connectionString, catalogName, project.ProjectId);

            var parameterDtos = parameters.Select(p => new ProjectParameterDto
            {
                Name = p.ParameterName,
                DataType = p.DataType,
                Required = p.Required,
                DefaultValue = p.DefaultValue,
                Description = p.Description
            }).ToList();

            _logger.LogInformation("Found package {Folder}/{Project}/{Package} with {Count} project parameters",
                folderName, projectName, packageName, parameterDtos.Count);

            return new PackageExistenceResultDto
            {
                Success = true,
                FolderExists = true,
                ProjectExists = true,
                PackageExists = true,
                Parameters = parameterDtos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking package existence");
            return new PackageExistenceResultDto
            {
                Success = false,
                ErrorMessage = $"Connection error: {ex.Message}",
                FolderExists = false,
                ProjectExists = false,
                PackageExists = false
            };
        }
    }

    public async Task<CatalogExecutionResultDto> ExecutePackageAsync(
        string serverAddress,
        string catalogName,
        bool useWindowsAuth,
        string? username,
        string? password,
        string folderName,
        string projectName,
        string packageName,
        Dictionary<string, object>? parameters = null)
    {
        var logs = new StringBuilder();

        try
        {
            var connectionString = useWindowsAuth
                ? $"Server={serverAddress};Database={catalogName};Integrated Security=true;TrustServerCertificate=true;"
                : $"Server={serverAddress};Database={catalogName};User ID={username};Password={password};TrustServerCertificate=true;";

            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Connecting to {catalogName} catalog");

            var folder = await _catalogRepository.GetFolderAsync(connectionString, catalogName, folderName);
            if (folder == null)
            {
                var errorMsg = $"Folder '{folderName}' not found in {catalogName} catalog";
                logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR: {errorMsg}");
                return new CatalogExecutionResultDto
                {
                    Success = false,
                    ExecutionId = 0,
                    Status = "Error",
                    Logs = logs.ToString(),
                    ErrorMessage = errorMsg
                };
            }

            var project = await _catalogRepository.GetProjectAsync(connectionString, catalogName, folder.FolderId, projectName);
            if (project == null)
            {
                var errorMsg = $"Project '{projectName}' not found in folder '{folderName}'";
                logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR: {errorMsg}");
                return new CatalogExecutionResultDto
                {
                    Success = false,
                    ExecutionId = 0,
                    Status = "Error",
                    Logs = logs.ToString(),
                    ErrorMessage = errorMsg
                };
            }

            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Connected to {catalogName} catalog");

            var executionId = await _catalogRepository.CreateExecutionAsync(connectionString, catalogName, project.ProjectId, packageName);
            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Execution created with ID: {executionId}");

            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    await _catalogRepository.SetExecutionParameterAsync(connectionString, executionId, param.Key, param.Value ?? "");

                    var logValue = param.Key.Contains("Password", StringComparison.OrdinalIgnoreCase)
                        ? "********"
                        : param.Value?.ToString();
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Parameter set: {param.Key} = {logValue}");
                }
            }

            await _catalogRepository.StartExecutionAsync(connectionString, executionId);
            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Execution started");

            string status = SSISExecutionStatus.Running.ToDisplayString();
            int waitCount = 0;
            int maxWaitSeconds = 300;

            while (status == SSISExecutionStatus.Running.ToDisplayString() ||
                   status == SSISExecutionStatus.Created.ToDisplayString() ||
                   status == SSISExecutionStatus.Pending.ToDisplayString())
            {
                await Task.Delay(2000);
                waitCount += 2;

                var executionModel = await _catalogRepository.GetExecutionStatusAsync(connectionString, executionId);
                if (executionModel != null)
                {
                    status = executionModel.Status.ToDisplayString();
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Status: {status}");
                }
                else
                {
                    status = SSISExecutionStatus.Failed.ToDisplayString();
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR: Could not retrieve execution status");
                    break;
                }

                if (waitCount >= maxWaitSeconds)
                {
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Timeout - execution still running");
                    return new CatalogExecutionResultDto
                    {
                        Success = false,
                        ExecutionId = executionId,
                        Status = "Timeout",
                        Logs = logs.ToString(),
                        ErrorMessage = "Execution timeout after 5 minutes"
                    };
                }
            }

            bool success = status == SSISExecutionStatus.Succeeded.ToDisplayString();
            string? errorMessage = success ? null : $"Execution completed with status: {status}";

            _logger.LogInformation("Package execution completed: {Status} (ID: {ExecutionId})", status, executionId);

            return new CatalogExecutionResultDto
            {
                Success = success,
                ExecutionId = executionId,
                Status = status,
                Logs = logs.ToString(),
                ErrorMessage = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing package");
            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}");
            return new CatalogExecutionResultDto
            {
                Success = false,
                ExecutionId = 0,
                Status = "Error",
                Logs = logs.ToString(),
                ErrorMessage = ex.Message
            };
        }
    }
}
