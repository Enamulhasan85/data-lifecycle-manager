using DataLifecycleManager.Domain.Enums;

namespace DataLifecycleManager.Presentation.ViewModels.DatabaseConnection
{
    /// <summary>
    /// View Model for displaying database connection details
    /// </summary>
    public class DatabaseConnectionViewModel
    {
        public int Id { get; set; }
        public string ConnectionName { get; set; } = string.Empty;
        public DatabaseProvider Provider { get; set; }
        public string ServerAddress { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public ConnectionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
