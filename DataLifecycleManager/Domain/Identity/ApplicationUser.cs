using Microsoft.AspNetCore.Identity;

namespace DataLifecycleManager.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string? ProfileImageUrl { get; set; }
    public string? Notes { get; set; }

    // TODO: Add navigation properties for Data Lifecycle Manager domain entities here
    // Example: public ICollection<DataArchiveJob> ArchivedJobs { get; set; }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void SetRefreshToken(string token, DateTime expiryTime)
    {
        RefreshToken = token;
        RefreshTokenExpiryTime = expiryTime;
    }

    public bool IsRefreshTokenValid()
    {
        return !string.IsNullOrEmpty(RefreshToken) &&
               RefreshTokenExpiryTime.HasValue &&
               RefreshTokenExpiryTime.Value > DateTime.UtcNow;
    }
}
