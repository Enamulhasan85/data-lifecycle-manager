namespace DataLifecycleManager.Application.DTOs.SSISPackage;

public class PackageExistenceResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool FolderExists { get; set; }
    public bool ProjectExists { get; set; }
    public bool PackageExists { get; set; }
    public List<ProjectParameterDto>? Parameters { get; set; }
}
