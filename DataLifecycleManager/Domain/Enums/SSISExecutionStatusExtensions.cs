namespace DataLifecycleManager.Domain.Enums;

public static class SSISExecutionStatusExtensions
{
    public static string ToDisplayString(this SSISExecutionStatus status)
    {
        return status switch
        {
            SSISExecutionStatus.Created => "Created",
            SSISExecutionStatus.Running => "Running",
            SSISExecutionStatus.Canceled => "Canceled",
            SSISExecutionStatus.Failed => "Failed",
            SSISExecutionStatus.Pending => "Pending",
            SSISExecutionStatus.EndedUnexpectedly => "Ended unexpectedly",
            SSISExecutionStatus.Succeeded => "Succeeded",
            SSISExecutionStatus.Stopping => "Stopping",
            SSISExecutionStatus.Completed => "Completed",
            _ => "Unknown"
        };
    }
}
