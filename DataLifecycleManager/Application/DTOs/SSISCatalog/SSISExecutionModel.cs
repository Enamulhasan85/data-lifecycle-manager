using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Application.DTOs.SSISCatalog;

public class SSISExecutionModel
{
    public long ExecutionId { get; set; }
    public SSISExecutionStatus Status { get; set; }
}
