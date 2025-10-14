using DataLifecycleManager.Application.DTOs.Common;
using DataLifecycleManager.Application.DTOs.UserManagement;

namespace DataLifecycleManager.Application.Interfaces
{
    /// <summary>
    /// Service interface for user management operations
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Get all users with their roles
        /// </summary>
        Task<Result<List<UserDto>>> GetAllUsersAsync();

        /// <summary>
        /// Get user by ID with detailed information
        /// </summary>
        Task<Result<UserDto>> GetUserByIdAsync(string userId);

        /// <summary>
        /// Create a new user
        /// </summary>
        Task<Result<string>> CreateUserAsync(CreateUserDto createUserDto, string createdBy);

        /// <summary>
        /// Update existing user information
        /// </summary>
        Task<Result<bool>> UpdateUserAsync(UpdateUserDto updateUserDto, string updatedBy);

        /// <summary>
        /// Deactivate user (soft delete)
        /// </summary>
        Task<Result<bool>> DeactivateUserAsync(string userId, string currentUserId, string deactivatedBy);

        /// <summary>
        /// Get all available roles
        /// </summary>
        Task<Result<List<string>>> GetAllRolesAsync();
    }
}
