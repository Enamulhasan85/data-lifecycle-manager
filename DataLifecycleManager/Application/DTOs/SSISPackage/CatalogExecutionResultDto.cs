namespace DataLifecycleManager.Application.DTOs.SSISPackage;

public class CatalogExecutionResultDto
{
    public bool Success { get; set; }
    public long ExecutionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Logs { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
