namespace DataLifecycleManager.Presentation.ViewModels.SSISPackageExecution;

/// <summary>
/// ViewModel for displaying SSIS package execution in a list
/// </summary>
public class ExecutionListViewModel
{
    public int Id { get; set; }
    public int SSISPackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public long? CatalogExecutionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ExecutedBy { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
