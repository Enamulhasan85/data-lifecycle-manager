namespace DataLifecycleManager.Domain.Identity;

/// <summary>
/// Constants for application role names
/// </summary>
public static class Roles
{
    public const string SystemAdmin = "System Admin";
    public const string ApplicationManager = "Application Manager";
    public const string DataAuditor = "Data Auditor";

    /// <summary>
    /// Roles that can perform write operations (Create, Update, Delete)
    /// </summary>
    public const string WriteRoles = SystemAdmin + "," + ApplicationManager;

    /// <summary>
    /// Roles that can perform read operations (View, List)
    /// </summary>
    public const string ReadRoles = SystemAdmin + "," + ApplicationManager + "," + DataAuditor;

    /// <summary>
    /// All application roles
    /// </summary>
    public static readonly string[] All = { SystemAdmin, ApplicationManager, DataAuditor };
}
