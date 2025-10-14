using DataLifecycleManager.Application.DTOs.Common;
using DataLifecycleManager.Application.DTOs.UserManagement;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataLifecycleManager.Application.Services
{
    /// <summary>
    /// Service implementation for user management operations
    /// Handles business logic for user CRUD operations
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .OrderBy(u => u.Email)
                    .ToListAsync();

                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(MapToUserDto(user, roles.ToList()));
                }

                return Result<List<UserDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return Result<List<UserDto>>.Failure("Failed to retrieve users");
            }
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result<UserDto>.Failure("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = MapToUserDto(user, roles.ToList());

                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return Result<UserDto>.Failure("Failed to retrieve user");
            }
        }

        public async Task<Result<string>> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = createUserDto.Email,
                    Email = createUserDto.Email,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    FullName = $"{createUserDto.FirstName} {createUserDto.LastName}",
                    PhoneNumber = createUserDto.PhoneNumber,
                    IsActive = createUserDto.IsActive,
                    RegistrationDate = DateTime.UtcNow,
                    EmailConfirmed = true // Auto-confirm for admin-created users
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<string>.Failure(errors);
                }

                // Assign roles
                if (createUserDto.SelectedRoles != null && createUserDto.SelectedRoles.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, createUserDto.SelectedRoles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to assign some roles to user {Email}", createUserDto.Email);
                    }
                }

                _logger.LogInformation("User {Email} was created by {CreatedBy}", createUserDto.Email, createdBy);
                return Result<string>.Success(user.Id, $"User '{createUserDto.Email}' created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", createUserDto.Email);
                return Result<string>.Failure("Failed to create user");
            }
        }

        public async Task<Result<bool>> UpdateUserAsync(UpdateUserDto updateUserDto, string updatedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updateUserDto.Id);
                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                user.FullName = $"{updateUserDto.FirstName} {updateUserDto.LastName}";
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.IsActive = updateUserDto.IsActive;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<bool>.Failure(errors);
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(updateUserDto.SelectedRoles ?? new List<string>()).ToList();
                var rolesToAdd = (updateUserDto.SelectedRoles ?? new List<string>()).Except(currentRoles).ToList();

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                _logger.LogInformation("User {Email} was updated by {UpdatedBy}", user.Email, updatedBy);
                return Result<bool>.Success(true, $"User '{user.Email}' updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", updateUserDto.Id);
                return Result<bool>.Failure("Failed to update user");
            }
        }

        public async Task<Result<bool>> DeactivateUserAsync(string userId, string currentUserId, string deactivatedBy)
        {
            try
            {
                // Prevent self-deletion
                if (userId == currentUserId)
                {
                    return Result<bool>.Failure("You cannot deactivate your own account");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                // Soft delete - deactivate user
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Result<bool>.Failure("Failed to deactivate user");
                }

                _logger.LogInformation("User {Email} was deactivated by {DeactivatedBy}", user.Email, deactivatedBy);
                return Result<bool>.Success(true, $"User '{user.Email}' deactivated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return Result<bool>.Failure("Failed to deactivate user");
            }
        }

        public async Task<Result<List<string>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToListAsync();

                return Result<List<string>>.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return Result<List<string>>.Failure("Failed to retrieve roles");
            }
        }

        #region Private Helpers

        private UserDto MapToUserDto(ApplicationUser user, List<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                LastLoginAt = user.LastLoginAt,
                Roles = roles,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber
            };
        }

        #endregion
    }
}
