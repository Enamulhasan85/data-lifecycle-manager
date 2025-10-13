using System.Collections.Generic;

namespace DataLifecycleManager.Application.Interfaces;

/// <summary>
/// Service to access current user context and information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current user's unique identifier
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Current user's username/email
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Current user's full name
    /// </summary>
    string? FullName { get; }

    /// <summary>
    /// Whether the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// User's assigned roles
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Check if user is in a specific role
    /// </summary>
    /// <param name="role">Role name to check</param>
    /// <returns>True if user is in role</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Check if user has any of the specified roles
    /// </summary>
    /// <param name="roles">Roles to check</param>
    /// <returns>True if user has any of the roles</returns>
    bool HasAnyRole(params string[] roles);

    /// <summary>
    /// Check if user has all of the specified roles
    /// </summary>
    /// <param name="roles">Roles to check</param>
    /// <returns>True if user has all roles</returns>
    bool HasAllRoles(params string[] roles);
}
