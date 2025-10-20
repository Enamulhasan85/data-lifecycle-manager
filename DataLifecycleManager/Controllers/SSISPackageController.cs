using AutoMapper;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Presentation.ViewModels.SSISPackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataLifecycleManager.Controllers;

[Authorize]
public class SSISPackageController : Controller
{
    private readonly ISSISPackageService _ssisPackageService;
    private readonly IMapper _mapper;
    private readonly ILogger<SSISPackageController> _logger;

    public SSISPackageController(
        ISSISPackageService ssisPackageService,
        IMapper mapper,
        ILogger<SSISPackageController> logger)
    {
        _ssisPackageService = ssisPackageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var packages = await _ssisPackageService.GetAllAsync();
        var viewModels = _mapper.Map<List<SSISPackageViewModel>>(packages);
        return View(viewModels);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<SSISPackageViewModel>(package);
        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSSISPackageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please correct the validation errors below.";
            return View(model);
        }

        try
        {
            var entity = _mapper.Map<SSISPackage>(model);
            await _ssisPackageService.CreateAsync(entity);

            TempData["SuccessMessage"] = $"SSIS Package '{model.PackageName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SSIS package");
            ModelState.AddModelError(string.Empty, "Error creating package. Folder/Project/Package combination must be unique.");
            ViewBag.ErrorMessage = "Error creating package. Folder/Project/Package combination must be unique.";
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<EditSSISPackageViewModel>(package);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSSISPackageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please correct the validation errors below.";
            return View(model);
        }

        try
        {
            var entity = await _ssisPackageService.GetByIdAsync(model.Id);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "SSIS Package not found.");
                ViewBag.ErrorMessage = "SSIS Package not found.";
                return View(model);
            }

            _mapper.Map(model, entity);
            await _ssisPackageService.UpdateAsync(model.Id, entity);

            TempData["SuccessMessage"] = $"SSIS Package '{model.PackageName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SSIS package");
            ModelState.AddModelError(string.Empty, "Error updating package.");
            ViewBag.ErrorMessage = "Error updating package.";
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<SSISPackageViewModel>(package);
        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var package = await _ssisPackageService.GetByIdAsync(id);
            if (package != null)
            {
                await _ssisPackageService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"SSIS Package '{package.PackageName}' deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "SSIS Package not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting SSIS Package: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> TestPackage(int id)
    {
        try
        {
            var (success, result) = await _ssisPackageService.TestPackageConnectionAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }

            if (result.Success)
            {
                var paramInfo = result.Parameters != null && result.Parameters.Any()
                    ? $"\n\nProject Parameters:\n{string.Join("\n", result.Parameters.Select(p => $"{p.Name} ({p.DataType}){(p.Required ? " *required" : "")}"))}"
                    : "\n\nNo project parameters found.";

                return Json(new
                {
                    success = true,
                    message = $"Package found in catalog!",
                    parameters = result.Parameters,
                    folderExists = result.FolderExists,
                    projectExists = result.ProjectExists,
                    packageExists = result.PackageExists
                });
            }

            return Json(new
            {
                success = false,
                message = result.ErrorMessage,
                folderExists = result.FolderExists,
                projectExists = result.ProjectExists,
                packageExists = result.PackageExists
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing SSIS package");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetPackageParameters(int id)
    {
        try
        {
            var (success, result) = await _ssisPackageService.TestPackageConnectionAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }

            return Json(result.Success
                ? new { success = true, parameters = result.Parameters }
                : new { success = false, message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project parameters");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExecutePackage(int id)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var (success, result) = await _ssisPackageService.ExecutePackageAsync(id);
            var duration = (int)(DateTime.UtcNow - startTime).TotalSeconds;

            if (!success)
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }

            return Json(result.Success
                ? new
                {
                    success = true,
                    message = $"Package executed successfully! (ID: {result.ExecutionId}, Duration: {duration}s)",
                    executionId = result.ExecutionId,
                    status = result.Status,
                    durationSeconds = duration
                }
                : new
                {
                    success = false,
                    message = $"Execution failed: {result.ErrorMessage}",
                    status = result.Status,
                    logs = result.Logs
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing SSIS package");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}
