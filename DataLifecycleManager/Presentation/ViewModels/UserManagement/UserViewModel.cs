using System.ComponentModel.DataAnnotations;

namespace DataLifecycleManager.Presentation.ViewModels.UserManagement
{
    /// <summary>
    /// View Model for displaying user information in views
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginAt { get; set; }

        [Display(Name = "Roles")]
        public string Roles { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
