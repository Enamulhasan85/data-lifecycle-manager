using System.ComponentModel.DataAnnotations;
using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Presentation.ViewModels.DatabaseConnection
{
    /// <summary>
    /// View Model for editing an existing database connection
    /// </summary>
    public class EditDatabaseConnectionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Connection name is required")]
        [Display(Name = "Connection Name")]
        [StringLength(200, ErrorMessage = "Connection name cannot exceed 200 characters")]
        public string ConnectionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provider is required")]
        [Display(Name = "Database Provider")]
        public DatabaseProvider Provider { get; set; }

        [Required(ErrorMessage = "Server address is required")]
        [Display(Name = "Server Address")]
        [StringLength(500, ErrorMessage = "Server address cannot exceed 500 characters")]
        public string ServerAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Database name is required")]
        [Display(Name = "Database Name")]
        [StringLength(200, ErrorMessage = "Database name cannot exceed 200 characters")]
        public string DatabaseName { get; set; } = string.Empty;

        [Display(Name = "Username")]
        [StringLength(200, ErrorMessage = "Username cannot exceed 200 characters")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(500, ErrorMessage = "Password cannot exceed 500 characters")]
        public string? Password { get; set; }

        [Display(Name = "Use Windows Authentication")]
        public bool UseWindowsAuthentication { get; set; }

        [Display(Name = "Status")]
        public ConnectionStatus Status { get; set; }

        // Audit properties for display
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
