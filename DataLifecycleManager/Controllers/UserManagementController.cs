using AutoMapper;
using DataLifecycleManager.Application.DTOs.UserManagement;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Presentation.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataLifecycleManager.Controllers
{
    /// <summary>
    /// Controller for managing users (restricted to System Admin and Application Manager roles)
    /// Thin controller - delegates business logic to Application layer
    /// </summary>
    [Authorize(Roles = "System Admin,Application Manager")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementService userManagementService,
            IMapper mapper,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Display list of all users
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _userManagementService.GetAllUsersAsync();

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new List<UserViewModel>());
            }

            var viewModels = _mapper.Map<List<UserViewModel>>(result.Value);
            return View(viewModels);
        }

        /// <summary>
        /// Display create user form
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var rolesResult = await _userManagementService.GetAllRolesAsync();
            ViewBag.Roles = rolesResult.Succeeded ? rolesResult.Value : new List<string>();
            return View();
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var rolesResult = await _userManagementService.GetAllRolesAsync();
                ViewBag.Roles = rolesResult.Succeeded ? rolesResult.Value : new List<string>();
                return View(model);
            }

            var createDto = _mapper.Map<CreateUserDto>(model);
            var result = await _userManagementService.CreateUserAsync(createDto, User.Identity?.Name ?? "Unknown");

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Add backend validation errors to ModelState
            ModelState.AddModelError(string.Empty, result.Message);
            ViewBag.ErrorMessage = result.Message; // Also pass as ViewBag for explicit display
            var rolesReload = await _userManagementService.GetAllRolesAsync();
            ViewBag.Roles = rolesReload.Succeeded ? rolesReload.Value : new List<string>();
            return View(model);
        }

        /// <summary>
        /// Display edit user form
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var result = await _userManagementService.GetUserByIdAsync(id);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = _mapper.Map<EditUserViewModel>(result.Value);
            var rolesResult = await _userManagementService.GetAllRolesAsync();
            ViewBag.Roles = rolesResult.Succeeded ? rolesResult.Value : new List<string>();
            return View(model);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var rolesResult = await _userManagementService.GetAllRolesAsync();
                ViewBag.Roles = rolesResult.Succeeded ? rolesResult.Value : new List<string>();
                return View(model);
            }

            var updateDto = _mapper.Map<UpdateUserDto>(model);
            var result = await _userManagementService.UpdateUserAsync(updateDto, User.Identity?.Name ?? "Unknown");

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Add backend validation errors to ModelState
            ModelState.AddModelError(string.Empty, result.Message);
            ViewBag.ErrorMessage = result.Message; // Also pass as ViewBag for explicit display
            var rolesReload = await _userManagementService.GetAllRolesAsync();
            ViewBag.Roles = rolesReload.Succeeded ? rolesReload.Value : new List<string>();
            return View(model);
        }

        /// <summary>
        /// Display user details
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var result = await _userManagementService.GetUserByIdAsync(id);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var model = _mapper.Map<UserViewModel>(result.Value);
            return View(model);
        }

        /// <summary>
        /// Delete user (soft delete by deactivating)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _userManagementService.DeactivateUserAsync(id, currentUserId, User.Identity?.Name ?? "Unknown");

            TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
