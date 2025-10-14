using System.ComponentModel.DataAnnotations;

namespace DataLifecycleManager.Application.DTOs.UserManagement
{
    /// <summary>
    /// Data Transfer Object for updating user information
    /// </summary>
    public class UpdateUserDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public List<string>? SelectedRoles { get; set; }
    }
}
