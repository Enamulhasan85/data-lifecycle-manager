namespace DataLifecycleManager.Presentation.ViewModels.SSISPackage;

public class ExecuteSSISPackageViewModel
{
    public int Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ServerAddress { get; set; } = string.Empty;
    public string CatalogName { get; set; } = string.Empty;
    public int TimeoutMinutes { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}
