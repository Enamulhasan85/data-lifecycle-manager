namespace DataLifecycleManager.Presentation.ViewModels.SSISPackageExecution;

/// <summary>
/// ViewModel for displaying detailed information about an SSIS package execution
/// </summary>
public class ExecutionDetailsViewModel
{
    public int Id { get; set; }
    public int SSISPackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ServerAddress { get; set; } = string.Empty;
    public string CatalogName { get; set; } = string.Empty;
    public long? CatalogExecutionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ExecutedBy { get; set; }
    public Dictionary<string, string> ExecutionParameters { get; set; } = new();
    public string? ExecutionLogs { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties
    public bool IsRunning => Status == "Running" || Status == "Created" || Status == "Pending";
    public bool IsCompleted => Status == "Succeeded" || Status == "Failed" || Status == "Cancelled" || Status == "Stopped";
    public bool IsSuccessful => Status == "Succeeded";
}
