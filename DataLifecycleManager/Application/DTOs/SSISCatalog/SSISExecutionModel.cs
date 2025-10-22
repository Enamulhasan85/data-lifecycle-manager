using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Application.DTOs.SSISCatalog;

public class SSISExecutionModel
{
    public long ExecutionId { get; set; }
    public SSISExecutionStatus Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Messages { get; set; } = new();
}
