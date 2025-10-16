using AutoMapper;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Presentation.ViewModels.DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataLifecycleManager.Controllers;

[Authorize]
public class DatabaseConnectionController : Controller
{
    private readonly IDatabaseConnectionService _connectionService;
    private readonly IMapper _mapper;
    private readonly ILogger<DatabaseConnectionController> _logger;

    public DatabaseConnectionController(
        IDatabaseConnectionService connectionService,
        IMapper mapper,
        ILogger<DatabaseConnectionController> logger)
    {
        _connectionService = connectionService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: DatabaseConnection
    public async Task<IActionResult> Index()
    {
        var connections = await _connectionService.GetAllAsync();
        var viewModels = _mapper.Map<List<DatabaseConnectionViewModel>>(connections);
        return View(viewModels);
    }

    // GET: DatabaseConnection/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DatabaseConnection/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDatabaseConnectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please correct the validation errors below.";
            return View(model);
        }

        try
        {
            if (await _connectionService.ConnectionNameExistsAsync(model.ConnectionName))
            {
                ModelState.AddModelError(nameof(model.ConnectionName), "Connection name already exists.");
                ViewBag.ErrorMessage = "Connection name already exists.";
                return View(model);
            }

            var entity = _mapper.Map<DatabaseConnection>(model);
            await _connectionService.CreateAsync(entity);

            TempData["SuccessMessage"] = $"Database connection '{model.ConnectionName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database connection");
            ModelState.AddModelError(string.Empty, "Error creating connection.");
            ViewBag.ErrorMessage = "Error creating connection.";
        }

        return View(model);
    }

    // GET: DatabaseConnection/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var connection = await _connectionService.GetByIdAsync(id.Value);
        if (connection == null) return NotFound();

        var viewModel = _mapper.Map<EditDatabaseConnectionViewModel>(connection);
        return View(viewModel);
    }

    // POST: DatabaseConnection/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditDatabaseConnectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please correct the validation errors below.";
            return View(model);
        }

        try
        {
            var entity = await _connectionService.GetByIdAsync(model.Id);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "Database connection not found.");
                ViewBag.ErrorMessage = "Database connection not found.";
                return View(model);
            }

            if (await _connectionService.ConnectionNameExistsAsync(model.ConnectionName, model.Id))
            {
                ModelState.AddModelError(nameof(model.ConnectionName), "Connection name already exists.");
                ViewBag.ErrorMessage = "Connection name already exists.";
                return View(model);
            }

            _mapper.Map(model, entity);
            await _connectionService.UpdateAsync(model.Id, entity);

            TempData["SuccessMessage"] = $"Database connection '{model.ConnectionName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating database connection");
            ModelState.AddModelError(string.Empty, "Error updating connection.");
            ViewBag.ErrorMessage = "Error updating connection.";
        }

        return View(model);
    }

    // GET: DatabaseConnection/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var connection = await _connectionService.GetByIdAsync(id.Value);
        if (connection == null) return NotFound();

        var viewModel = _mapper.Map<DatabaseConnectionViewModel>(connection);
        return View(viewModel);
    }

    // GET: DatabaseConnection/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var connection = await _connectionService.GetByIdAsync(id.Value);
        if (connection == null) return NotFound();

        var viewModel = _mapper.Map<DatabaseConnectionViewModel>(connection);
        return View(viewModel);
    }

    // POST: DatabaseConnection/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var connection = await _connectionService.GetByIdAsync(id);
            if (connection != null)
            {
                await _connectionService.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Database connection '{connection.ConnectionName}' deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Database connection not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting database connection: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: DatabaseConnection/TestConnection/5
    [HttpPost]
    public async Task<IActionResult> TestConnection(int id)
    {
        try
        {
            var connection = await _connectionService.GetByIdAsync(id);
            if (connection == null)
            {
                return Json(new { success = false, message = "Connection not found." });
            }

            var isValid = await _connectionService.TestConnectionAsync(id);

            if (isValid)
            {
                return Json(new { success = true, message = "Connection test successful!" });
            }
            else
            {
                return Json(new { success = false, message = "Check your connection parameters." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing database connection");
            return Json(new { success = false, message = $"Error testing connection: {ex.Message}" });
        }
    }

}
