using System.Data;
using DataLifecycleManager.Application.DTOs.SSISCatalog;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataLifecycleManager.Infrastructure.Data.Repositories;

public class SSISCatalogRepository : ISSISCatalogRepository
{
    private readonly ILogger<SSISCatalogRepository> _logger;

    public SSISCatalogRepository(ILogger<SSISCatalogRepository> logger)
    {
        _logger = logger;
    }

    public async Task<SSISFolderModel?> GetFolderAsync(string connectionString, string catalogName, string folderName)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                "SELECT folder_id, name FROM [catalog].[folders] WHERE name = @folder_name",
                connection);

            cmd.Parameters.AddWithValue("@folder_name", folderName);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SSISFolderModel
                {
                    FolderId = reader.GetInt64(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting folder {FolderName} from catalog {CatalogName}", folderName, catalogName);
            throw;
        }
    }

    public async Task<SSISProjectModel?> GetProjectAsync(string connectionString, string catalogName, long folderId, string projectName)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                @"SELECT project_id, folder_id, name 
                  FROM [catalog].[projects] 
                  WHERE folder_id = @folder_id AND name = @project_name",
                connection);

            cmd.Parameters.AddWithValue("@folder_id", folderId);
            cmd.Parameters.AddWithValue("@project_name", projectName);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SSISProjectModel
                {
                    ProjectId = reader.GetInt64(0),
                    FolderId = reader.GetInt64(1),
                    Name = reader.GetString(2)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project {ProjectName} from folder ID {FolderId}", projectName, folderId);
            throw;
        }
    }

    public async Task<SSISPackageModel?> GetPackageAsync(string connectionString, string catalogName, long projectId, string packageName)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                @"SELECT package_id, project_id, name 
                  FROM [catalog].[packages] 
                  WHERE project_id = @project_id AND name = @package_name",
                connection);

            cmd.Parameters.AddWithValue("@project_id", projectId);
            cmd.Parameters.AddWithValue("@package_name", packageName);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SSISPackageModel
                {
                    PackageId = reader.GetInt64(0),
                    ProjectId = reader.GetInt64(1),
                    Name = reader.GetString(2)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting package {PackageName} from project ID {ProjectId}", packageName, projectId);
            throw;
        }
    }

    public async Task<List<SSISParameterModel>> GetProjectParametersAsync(string connectionString, string catalogName, long projectId)
    {
        try
        {
            var parameters = new List<SSISParameterModel>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                @"SELECT 
                    parameter_name,
                    data_type,
                    required,
                    default_value,
                    description
                  FROM [catalog].[object_parameters]
                  WHERE project_id = @project_id 
                    AND object_type = @object_type
                    AND required = 1
                  ORDER BY parameter_name",
                connection);

            cmd.Parameters.AddWithValue("@project_id", projectId);
            cmd.Parameters.AddWithValue("@object_type", (int)SSISObjectType.Project);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                parameters.Add(new SSISParameterModel
                {
                    ParameterName = reader.GetString(0),
                    DataType = reader.GetString(1),
                    Required = reader.GetBoolean(2),
                    DefaultValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Description = reader.IsDBNull(4) ? null : reader.GetString(4)
                });
            }

            return parameters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project parameters for project ID {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<long> CreateExecutionAsync(string connectionString, string catalogName, long projectId, string packageName)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var projectCmd = new SqlCommand(
                @"SELECT f.name, p.name
                  FROM [catalog].[projects] p
                  JOIN [catalog].[folders] f ON p.folder_id = f.folder_id
                  WHERE p.project_id = @project_id",
                connection);

            projectCmd.Parameters.AddWithValue("@project_id", projectId);

            string? folderName = null;
            string? projectName = null;

            using (var reader = await projectCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    folderName = reader.GetString(0);
                    projectName = reader.GetString(1);
                }
            }

            if (folderName == null || projectName == null)
            {
                throw new InvalidOperationException($"Project ID {projectId} not found");
            }

            using var cmd = new SqlCommand("[catalog].[create_execution]", connection);
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
            return (long)executionIdParam.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating execution for project ID {ProjectId}, package {PackageName}", projectId, packageName);
            throw;
        }
    }

    public async Task SetExecutionParameterAsync(string connectionString, long executionId, string parameterName, object value)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand("[catalog].[set_execution_parameter_value]", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@execution_id", executionId);
            cmd.Parameters.AddWithValue("@object_type", (int)SSISObjectType.Project);
            cmd.Parameters.AddWithValue("@parameter_name", parameterName);
            cmd.Parameters.AddWithValue("@parameter_value", value?.ToString() ?? "");

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting parameter {ParameterName} for execution ID {ExecutionId}", parameterName, executionId);
            throw;
        }
    }

    public async Task StartExecutionAsync(string connectionString, long executionId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand("[catalog].[start_execution]", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@execution_id", executionId);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting execution ID {ExecutionId}", executionId);
            throw;
        }
    }

    public async Task<SSISExecutionModel?> GetExecutionStatusAsync(string connectionString, long executionId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                @"SELECT 
                    execution_id, 
                    status, 
                    start_time, 
                    end_time,
                    (SELECT TOP 1 message FROM [catalog].[operation_messages] 
                     WHERE operation_id = execution_id 
                     AND message_type IN (120, 130) -- Error messages
                     ORDER BY message_time DESC) as error_message
                  FROM [catalog].[executions] 
                  WHERE execution_id = @execution_id",
                connection);

            cmd.Parameters.AddWithValue("@execution_id", executionId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var model = new SSISExecutionModel
                {
                    ExecutionId = reader.GetInt64(0),
                    Status = (SSISExecutionStatus)reader.GetInt32(1),
                    StartTime = reader.IsDBNull(2) ? null : reader.GetDateTimeOffset(2).UtcDateTime,
                    EndTime = reader.IsDBNull(3) ? null : reader.GetDateTimeOffset(3).UtcDateTime,
                    ErrorMessage = reader.IsDBNull(4) ? null : reader.GetString(4)
                };

                // Get execution messages
                model.Messages = await GetExecutionMessagesAsync(connectionString, executionId);

                return model;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting execution status for execution ID {ExecutionId}", executionId);
            throw;
        }
    }

    public async Task<List<string>> GetExecutionMessagesAsync(string connectionString, long executionId)
    {
        var messages = new List<string>();

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(
                @"SELECT 
                    message_time,
                    message_type,
                    message_source_type,
                    message
                  FROM [catalog].[operation_messages]
                  WHERE operation_id = @execution_id
                    AND message_type IN (50,60,70,80,90,100,110,120,130)
                  ORDER BY message_time ASC",
                connection);

            cmd.Parameters.AddWithValue("@execution_id", executionId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var messageTime = reader.GetDateTimeOffset(0).ToString("yyyy-MM-dd HH:mm:ss");
                var messageType = reader.GetInt16(1);
                var messageSourceType = reader.IsDBNull(2) ? 0 : reader.GetInt16(2);
                var message = reader.IsDBNull(3) ? "" : reader.GetString(3);

                // Format message type for readability
                var messageTypeStr = messageType switch
                {
                    120 => "ERROR",
                    110 => "WARNING",
                    70 => "INFORMATION",
                    10 => "PRE-VALIDATE",
                    20 => "POST-VALIDATE",
                    30 => "PRE-EXECUTE",
                    40 => "POST-EXECUTE",
                    50 => "STATUSCHANGE",
                    60 => "PROGRESS",
                    80 => "QUERYCANCEL",
                    90 => "TASKFAILED",
                    100 => "DIAGNOSTIC",
                    130 => "DIAGERROR",
                    _ => $"TYPE_{messageType}"
                };

                // Build formatted message
                var formattedMessage = $"[{messageTime}] [{messageTypeStr}]";
                formattedMessage += $" {message}";
                formattedMessage += $" [{messageSourceType}]";

                messages.Add(formattedMessage);
            }

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting execution messages for execution ID {ExecutionId}", executionId);
            // Return empty list instead of throwing to allow execution to continue
            return messages;
        }
    }
}
