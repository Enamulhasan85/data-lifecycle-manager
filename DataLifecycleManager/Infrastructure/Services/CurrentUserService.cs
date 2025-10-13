using System.Security.Claims;
using DataLifecycleManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DataLifecycleManager.Infrastructure.Services
{
    /// <summary>
    /// Service for accessing current user information from HTTP context
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("FullName")
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value) ?? Enumerable.Empty<string>();

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public bool HasAnyRole(params string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return false;

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return false;

            return roles.Any(role => user.IsInRole(role));
        }

        public bool HasAllRoles(params string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return true;

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return false;

            return roles.All(role => user.IsInRole(role));
        }
    }
}
