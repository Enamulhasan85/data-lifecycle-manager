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

    public SSISPackageController(ISSISPackageService ssisPackageService, IMapper mapper, ILogger<SSISPackageController> logger)
    {
        _ssisPackageService = ssisPackageService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: SSISPackage
    public async Task<IActionResult> Index()
    {
        var packages = await _ssisPackageService.GetPackagesWithConnectionsAsync();
        var viewModels = _mapper.Map<List<SSISPackageViewModel>>(packages);
        return View(viewModels);
    }

    // GET: SSISPackage/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<SSISPackageViewModel>(package);
        return View(viewModel);
    }

    // GET: SSISPackage/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SSISPackage/Create
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

    // GET: SSISPackage/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<EditSSISPackageViewModel>(package);
        return View(viewModel);
    }

    // POST: SSISPackage/Edit/5
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

    // GET: SSISPackage/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var viewModel = _mapper.Map<SSISPackageViewModel>(package);
        return View(viewModel);
    }

    // POST: SSISPackage/Delete/5
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

    // GET: SSISPackage/AssignConnections/5
    public async Task<IActionResult> AssignConnections(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var assignedConnectionIds = await _ssisPackageService.GetAssignedConnectionIdsAsync(id.Value);
        var connections = await _ssisPackageService.GetActiveConnectionsAsync();

        ViewBag.AssignedConnectionIds = assignedConnectionIds;
        ViewBag.Connections = connections;

        return View(package);
    }

    // POST: SSISPackage/AssignConnections
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignConnections(int packageId, List<int> selectedConnectionIds)
    {
        var package = await _ssisPackageService.GetByIdAsync(packageId);
        if (package == null) return NotFound();

        try
        {
            await _ssisPackageService.AssignConnectionsAsync(packageId, selectedConnectionIds ?? new List<int>());

            TempData["SuccessMessage"] = $"{selectedConnectionIds?.Count ?? 0} database connection(s) assigned to package '{package.PackageName}'.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning connections");
            TempData["ErrorMessage"] = "Error assigning connections.";

            // Re-populate ViewBag data for the view
            var assignedConnectionIds = await _ssisPackageService.GetAssignedConnectionIdsAsync(packageId);
            var connections = await _ssisPackageService.GetActiveConnectionsAsync();
            ViewBag.AssignedConnectionIds = assignedConnectionIds;
            ViewBag.Connections = connections;
            ViewBag.ErrorMessage = "Error assigning connections.";

            return View(package);
        }
    }

    // GET: SSISPackage/ViewConnections/5
    public async Task<IActionResult> ViewConnections(int? id)
    {
        if (id == null) return NotFound();

        var package = await _ssisPackageService.GetByIdAsync(id.Value);
        if (package == null) return NotFound();

        var connectionPackages = await _ssisPackageService.GetConnectionPackagesAsync(id.Value);

        ViewBag.Package = package;
        return View(connectionPackages);
    }
}
