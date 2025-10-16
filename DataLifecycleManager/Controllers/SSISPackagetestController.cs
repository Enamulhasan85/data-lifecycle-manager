using System.Data;
using System.Text;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DataLifecycleManager.Controllers;

/// <summary>
/// Controller for SSIS package execution from catalog
/// </summary>
[Authorize]
public class SSISPackagetestController : Controller
{
    private readonly ILogger<SSISPackagetestController> _logger;
    private readonly string _ssisdbConnectionString = "Server=HANNAHSTATION;Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;";

    public SSISPackagetestController(ILogger<SSISPackagetestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Display SSIS execution page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Execute SSIS package from SSIS Catalog
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RunFromCatalog(
        string folderName,
        string projectName,
        string packageName,
        string srcServer,
        string srcDatabase,
        string srcUser,
        string srcPassword,
        string destServer,
        string destDatabase,
        string destUser,
        string destPassword)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(folderName) ||
                string.IsNullOrWhiteSpace(projectName) ||
                string.IsNullOrWhiteSpace(packageName))
            {
                return Json(new
                {
                    success = false,
                    errorMessage = "Folder name, project name, and package name are required"
                });
            }

            // Validate parameters
            if (string.IsNullOrWhiteSpace(srcServer) || string.IsNullOrWhiteSpace(srcDatabase) ||
                string.IsNullOrWhiteSpace(destServer) || string.IsNullOrWhiteSpace(destDatabase))
            {
                return Json(new
                {
                    success = false,
                    errorMessage = "Source and destination server/database parameters are required"
                });
            }

            _logger.LogInformation("Executing SSIS package: {Folder}/{Project}/{Package}",
                folderName, projectName, packageName);

            var parameters = new Dictionary<string, object>
            {
                { "SrcServer", srcServer },
                { "SrcDatabase", srcDatabase },
                { "SrcUser", srcUser ?? "" },
                { "SrcPassword", srcPassword ?? "" },
                { "DestServer", destServer },
                { "DestDatabase", destDatabase },
                { "DestUser", destUser ?? "" },
                { "DestPassword", destPassword ?? "" }
            };

            var startTime = DateTime.UtcNow;
            var result = await ExecuteSSISPackageFromCatalog(folderName, projectName, packageName, parameters);
            var duration = (int)(DateTime.UtcNow - startTime).TotalSeconds;

            // Save execution record
            var execution = new SSISPackageExecution
            {
                Status = result.Success ? ExecutionStatus.Succeeded : ExecutionStatus.Failed,
                StartTime = startTime,
                EndTime = DateTime.UtcNow,
                DurationSeconds = duration,
                ExecutedBy = User.Identity?.Name,
                ExecutionLogs = result.Logs,
                ErrorMessage = result.ErrorMessage,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name
            };

            return Json(new
            {
                success = result.Success,
                executionId = result.ExecutionId,
                status = result.Status,
                logs = result.Logs,
                errorMessage = result.ErrorMessage,
                durationSeconds = duration
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SSIS package from catalog");
            return Json(new
            {
                success = false,
                errorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Execute SSIS package from SSIS Catalog
    /// </summary>
    private async Task<CatalogExecutionResult> ExecuteSSISPackageFromCatalog(
        string folderName,
        string projectName,
        string packageName,
        Dictionary<string, object>? parameters = null)
    {
        var logs = new StringBuilder();

        try
        {
            using var connection = new SqlConnection(_ssisdbConnectionString);
            await connection.OpenAsync();

            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Connected to SSISDB catalog");

            // Create execution
            long executionId;
            using (var cmd = new SqlCommand("[catalog].[create_execution]", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@folder_name", folderName);
                cmd.Parameters.AddWithValue("@project_name", projectName);
                cmd.Parameters.AddWithValue("@package_name", packageName);
                cmd.Parameters.AddWithValue("@use32bitruntime", false);
                cmd.Parameters.AddWithValue("@reference_id", DBNull.Value);

                var executionIdParam = new SqlParameter("@execution_id", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(executionIdParam);

                await cmd.ExecuteNonQueryAsync();
                executionId = (long)executionIdParam.Value;
                logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Execution created with ID: {executionId}");
            }

            // Set parameters if provided
            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    using var cmd = new SqlCommand("[catalog].[set_execution_parameter_value]", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@execution_id", executionId);
                    cmd.Parameters.AddWithValue("@object_type", 20); // 20 = Package parameter, 30 = Project parameter
                    cmd.Parameters.AddWithValue("@parameter_name", param.Key);
                    cmd.Parameters.AddWithValue("@parameter_value", param.Value);

                    await cmd.ExecuteNonQueryAsync();

                    // Log parameter (mask password fields)
                    var logValue = param.Key.Contains("Password", StringComparison.OrdinalIgnoreCase)
                        ? "********"
                        : param.Value.ToString();
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Parameter set: {param.Key} = {logValue}");
                }
            }

            // Start execution
            using (var cmd = new SqlCommand("[catalog].[start_execution]", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@execution_id", executionId);
                await cmd.ExecuteNonQueryAsync();
                logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Execution started");
            }

            // Monitor execution status
            string status = "Running";
            int waitCount = 0;
            int maxWaitSeconds = 300; // 5 minutes timeout

            while (status == "Running" || status == "Created" || status == "Pending")
            {
                await Task.Delay(2000); // Poll every 2 seconds
                waitCount += 2;

                using (var cmd = new SqlCommand(
                    "SELECT status FROM [catalog].[executions] WHERE execution_id = @execution_id",
                    connection))
                {
                    cmd.Parameters.AddWithValue("@execution_id", executionId);
                    var result = await cmd.ExecuteScalarAsync();
                    var statusCode = result != null ? (int)result : 4; // Default to Failed
                    status = GetStatusName(statusCode);
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Status: {status}");
                }

                if (waitCount >= maxWaitSeconds)
                {
                    logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Timeout - execution still running");
                    return new CatalogExecutionResult
                    {
                        Success = false,
                        ExecutionId = executionId,
                        Status = "Timeout",
                        Logs = logs.ToString(),
                        ErrorMessage = "Execution timeout after 5 minutes"
                    };
                }
            }

            // Get execution logs
            using (var cmd = new SqlCommand(
                @"SELECT TOP 100 message_time, message_type, message 
                  FROM [catalog].[operation_messages] 
                  WHERE operation_id = @execution_id 
                  ORDER BY message_time DESC",
                connection))
            {
                cmd.Parameters.AddWithValue("@execution_id", executionId);
                using var reader = await cmd.ExecuteReaderAsync();
                logs.AppendLine($"\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Execution Messages:");
                while (await reader.ReadAsync())
                {
                    var messageTime = reader.GetDateTime(0);
                    var messageType = reader.GetInt16(1);
                    var message = reader.GetString(2);
                    logs.AppendLine($"[{messageTime:yyyy-MM-dd HH:mm:ss}] [{GetMessageTypeName(messageType)}] {message}");
                }
            }

            bool success = status == "Succeeded";
            string? errorMessage = success ? null : $"Execution completed with status: {status}";

            return new CatalogExecutionResult
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
            logs.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR: {ex.Message}");
            return new CatalogExecutionResult
            {
                Success = false,
                ExecutionId = 0,
                Status = "Error",
                Logs = logs.ToString(),
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Get status name from code
    /// </summary>
    private string GetStatusName(int statusCode)
    {
        return statusCode switch
        {
            1 => "Created",
            2 => "Running",
            3 => "Canceled",
            4 => "Failed",
            5 => "Pending",
            6 => "Ended unexpectedly",
            7 => "Succeeded",
            8 => "Stopping",
            9 => "Completed",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Get message type name
    /// </summary>
    private string GetMessageTypeName(short messageType)
    {
        return messageType switch
        {
            120 => "Error",
            110 => "Warning",
            70 => "Information",
            _ => "Info"
        };
    }

    /// <summary>
    /// Catalog execution result
    /// </summary>
    private class CatalogExecutionResult
    {
        public bool Success { get; set; }
        public long ExecutionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Logs { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
