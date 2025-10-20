namespace DataLifecycleManager.Application.DTOs.SSISCatalog;

public class SSISParameterModel
{
    public string ParameterName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
    public string? Description { get; set; }
}
