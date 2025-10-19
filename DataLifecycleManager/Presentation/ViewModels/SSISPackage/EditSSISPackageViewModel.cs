using System.ComponentModel.DataAnnotations;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Presentation.ViewModels.SSISPackage
{
    /// <summary>
    /// View Model for editing an existing SSIS package
    /// </summary>
    public class EditSSISPackageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Folder name is required")]
        [Display(Name = "Folder Name")]
        [StringLength(200, ErrorMessage = "Folder name cannot exceed 200 characters")]
        public string FolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project name is required")]
        [Display(Name = "Project Name")]
        [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Package name is required")]
        [Display(Name = "Package Name")]
        [StringLength(200, ErrorMessage = "Package name cannot exceed 200 characters")]
        public string PackageName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public SSISPackageStatus Status { get; set; }

        [Required(ErrorMessage = "Timeout is required")]
        [Display(Name = "Timeout (Minutes)")]
        [Range(1, 1440, ErrorMessage = "Timeout must be between 1 and 1440 minutes")]
        public int TimeoutMinutes { get; set; }

        [Required(ErrorMessage = "Server address is required")]
        [Display(Name = "SQL Server Address")]
        [StringLength(500, ErrorMessage = "Server address cannot exceed 500 characters")]
        public string ServerAddress { get; set; } = string.Empty;

        [Display(Name = "Username")]
        [StringLength(200, ErrorMessage = "Username cannot exceed 200 characters")]
        public string? Username { get; set; }

        [Display(Name = "Password")]
        [StringLength(500, ErrorMessage = "Password cannot exceed 500 characters")]
        [DataType(DataType.Password)]
        public string? EncryptedPassword { get; set; }

        [Display(Name = "Use Windows Authentication")]
        public bool UseWindowsAuthentication { get; set; }

        [Display(Name = "Package Parameters (JSON)")]
        public string? PackageParameters { get; set; }
    }
}
