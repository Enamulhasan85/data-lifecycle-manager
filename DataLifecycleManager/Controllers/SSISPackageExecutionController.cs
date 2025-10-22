using AutoMapper;
using DataLifecycleManager.Application.Interfaces;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Domain.Identity;
using DataLifecycleManager.Presentation.ViewModels.SSISPackageExecution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataLifecycleManager.Controllers;

[Authorize(Roles = Roles.ReadRoles)]
public class SSISPackageExecutionController : Controller
{
    private readonly IRepository<SSISPackageExecution, int> _executionRepository;
    private readonly ISSISPackageService _ssisPackageService;
    private readonly IMapper _mapper;
    private readonly ILogger<SSISPackageExecutionController> _logger;

    public SSISPackageExecutionController(
        IRepository<SSISPackageExecution, int> executionRepository,
        ISSISPackageService ssisPackageService,
        IMapper mapper,
        ILogger<SSISPackageExecutionController> logger)
    {
        _executionRepository = executionRepository;
        _ssisPackageService = ssisPackageService;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: SSISPackageExecution
    public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
    {
        var paginatedResult = await _executionRepository.GetPaginatedAsync<ExecutionListViewModel>(
            page: page,
            pageSize: pageSize,
            selector: e => _mapper.Map<ExecutionListViewModel>(e),
            orderBy: e => e.CreatedAt,
            orderByDescending: true,
            includes: e => e.SSISPackage
        );

        var viewModels = _mapper.Map<List<ExecutionListViewModel>>(paginatedResult.Items);

        var pageViewModel = new ExecutionListPageViewModel
        {
            Executions = viewModels,
            CurrentPage = paginatedResult.PageNumber,
            PageSize = paginatedResult.PageSize,
            TotalCount = paginatedResult.TotalCount,
            TotalPages = paginatedResult.TotalPages,
            HasNextPage = paginatedResult.HasNextPage,
            HasPreviousPage = paginatedResult.HasPreviousPage
        };

        return View(pageViewModel);
    }

    // GET: SSISPackageExecution/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var execution = (await _executionRepository.FindAsync(
            predicate: e => e.Id == id,
            includes: e => e.SSISPackage
        )).FirstOrDefault();

        if (execution == null)
        {
            return NotFound();
        }

        var viewModel = _mapper.Map<ExecutionDetailsViewModel>(execution);
        return View(viewModel);
    }

    // POST: SSISPackageExecution/UpdateStatus/5
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        try
        {
            var success = await _ssisPackageService.UpdateExecutionStatusAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = "Failed to update execution status" });
            }

            var execution = (await _executionRepository.FindAsync(
                predicate: e => e.Id == id,
                includes: e => e.SSISPackage
            )).FirstOrDefault();

            if (execution == null)
            {
                return Json(new { success = false, message = "Execution not found" });
            }

            var viewModel = _mapper.Map<ExecutionDetailsViewModel>(execution);

            return Json(new
            {
                success = true,
                status = viewModel.Status,
                isRunning = viewModel.IsRunning,
                isCompleted = viewModel.IsCompleted,
                isSuccessful = viewModel.IsSuccessful,
                endTime = viewModel.EndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                durationSeconds = viewModel.DurationSeconds,
                executionLogs = viewModel.ExecutionLogs,
                errorMessage = viewModel.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating execution status for ID: {Id}", id);
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}
