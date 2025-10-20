using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Presentation.ViewModels.SSISPackage
{
    /// <summary>
    /// View Model for displaying SSIS package details
    /// </summary>
    public class SSISPackageViewModel
    {
        public int Id { get; set; }
        public string FolderName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SSISPackageStatus Status { get; set; }
        public int TimeoutMinutes { get; set; }
        public string ServerAddress { get; set; } = string.Empty;
        public string CatalogName { get; set; } = "SSISDB";
        public string? Username { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public string? PackageParameters { get; set; }
        public DateTime? LastExecutionDate { get; set; }
        public ExecutionStatus? LastExecutionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
