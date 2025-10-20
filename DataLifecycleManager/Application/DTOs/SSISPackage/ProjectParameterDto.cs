namespace DataLifecycleManager.Application.DTOs.SSISPackage;

public class ProjectParameterDto
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
    public string? Description { get; set; }
}
