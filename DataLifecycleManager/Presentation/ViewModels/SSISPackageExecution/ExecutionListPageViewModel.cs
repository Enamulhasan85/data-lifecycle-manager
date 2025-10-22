namespace DataLifecycleManager.Presentation.ViewModels.SSISPackageExecution;

/// <summary>
/// ViewModel for displaying paginated execution list
/// </summary>
public class ExecutionListPageViewModel
{
    public List<ExecutionListViewModel> Executions { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
