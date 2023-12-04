using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskApp.Models;
using TaskApp.Data;
using System.Diagnostics;

[Authorize]
public class TaskController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TaskController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: Task
    public async Task<IActionResult> Index(int? page, string filterDescription)
    {
        const int pageSize = 10; 

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        IQueryable<TaskApp.Models.Task> tasksQuery = _context.Tasks.Where(t => t.UserId == user.Id);

        if (!string.IsNullOrWhiteSpace(filterDescription))
        {
            tasksQuery = tasksQuery.Where(t => t.Description.Contains(filterDescription));
        }

        int pageNumber = page ?? 1;
        var tasks = await tasksQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        int totalItems = await tasksQuery.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var viewModel = new TaskViewModel
        {
            Tasks = tasks,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            FilterDescription = filterDescription
        };

        return View(viewModel);
    }


    // GET: Task/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Task/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Description,Date")] TaskApp.Models.Task task)
    {

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            task.UserId = user.Id;

            _context.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: Task/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return View(task);
    }

    // POST: Task/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Date")] TaskApp.Models.Task task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: Task/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(m => m.Id == id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // POST: Task/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Tasks/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var taskModel = await _context.Tasks
            .Include(t => t.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (taskModel == null)
        {
            return NotFound();
        }

        return View(taskModel);
    }



    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
}
