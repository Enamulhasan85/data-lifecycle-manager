using System.ComponentModel.DataAnnotations;

namespace DataLifecycleManager.Application.DTOs.UserManagement
{
    /// <summary>
    /// Data Transfer Object for creating a new user
    /// </summary>
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<string>? SelectedRoles { get; set; }
    }
}
