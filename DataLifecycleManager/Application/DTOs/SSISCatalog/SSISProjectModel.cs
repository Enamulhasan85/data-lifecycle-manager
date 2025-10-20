namespace DataLifecycleManager.Application.DTOs.SSISCatalog;

public class SSISProjectModel
{
    public long ProjectId { get; set; }
    public long FolderId { get; set; }
    public string Name { get; set; } = string.Empty;
}
