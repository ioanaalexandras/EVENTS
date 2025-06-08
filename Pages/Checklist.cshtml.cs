
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesEvents.Models;

namespace RazorPagesEvents.Pages;

public class ChecklistModel : PageModel
{
    private readonly EventDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ChecklistModel(EventDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<EventTask> EventTasks { get; set; } = new();
    public Dictionary<string, List<EventTask>> TasksByCategory { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int EventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedCategory { get; set; }

    public List<string> AllCategories { get; set; } = new();
    public bool CanAssignRoles { get; set; }
    public List<EventUserAccess> EventUsersWithAccess { get; set; } = new();

    public decimal TotalCost => TasksByCategory
        .SelectMany(c => c.Value)
        .Where(et => et.Cost.HasValue)
        .Sum(et => et.Cost ?? 0);

    public string EventName { get; set; } = "Eveniment";
    [BindProperty]
    public int TaskId { get; set; }
    [BindProperty]
    public string? UserId { get; set; }

    public Dictionary<int, List<PhotoGallery>> PhotosByTask { get; set; } = new();

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        var currentUserId = _userManager.GetUserId(User);

        var selectedEvent = await _context.Events
            .Include(e => e.Moneda)
            .FirstOrDefaultAsync(e => e.Id == EventId);
        if (selectedEvent == null)
        {
            NotFound();
            return;
        }

        ViewData["MonedaSimbol"] = selectedEvent?.Moneda?.Simbol ?? "RON";
        EventName = selectedEvent?.Description ?? "Eveniment";

        bool isOwner = selectedEvent?.UserId == currentUserId;
        CanAssignRoles = isOwner;

        IQueryable<EventTask> allTasks = _context.EventTasks
            .Include(et => et.Task).ThenInclude(t => t.TaskCategory)
            .Include(et => et.Status)
            .Where(et => et.EventId == EventId);

        if (!isOwner)
        {
            allTasks = allTasks.Where(et => et.UserId == currentUserId);
        }

        EventTasks = await allTasks.ToListAsync();

        var grouped = EventTasks
            .Where(et => et.Task.TaskCategory != null)
            .GroupBy(et => new
            {
                Id = et.Task.TaskCategory!.Id,
                Name = et.Task.TaskCategory.Name
            })
            .OrderBy(g => g.Key.Id)
            .ToList();

        AllCategories = grouped.Select(g => g.Key.Name).ToList();

        if (!string.IsNullOrEmpty(SelectedCategory))
        {
            TasksByCategory = grouped
                .Where(g => g.Key.Name == SelectedCategory)
                .ToDictionary(g => g.Key.Name, g => g.ToList());
        }
        else if (grouped.Any())
        {
            // ✅ Selectează implicit prima categorie
            var firstCategory = grouped.First();
            SelectedCategory = firstCategory.Key.Name;
            TasksByCategory = new Dictionary<string, List<EventTask>>
            {
                { firstCategory.Key.Name, firstCategory.ToList() }
            };
        }
        else
        {
            TasksByCategory = new(); // fallback dacă nu există taskuri deloc
        }

        // Lista utilizatori pentru dropdown (doar pentru creator)
        if (isOwner)
        {
            EventUsersWithAccess = await _context.UserAccesses
                .Include(a => a.User)
                .Where(a => a.EventId == EventId)
                .ToListAsync();
        }

        var taskIds = EventTasks.Select(et => et.Id).ToList();

        var allPhotos = await _context.PhotoGallery
            .Where(p => taskIds.Contains(p.EventTaskId) &&
                    (p.IsPublic || p.UserId == currentUserId || p.EventTask.UserId == currentUserId || isOwner)) // vezi doar poze proprii sau publice
            .ToListAsync();

        PhotosByTask = allPhotos
            .GroupBy(p => p.EventTaskId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<IActionResult> OnPostAsync(List<int> doneTaskIds, List<int> CostTaskIds, List<decimal> CostValues)
    {
        var action = Request.Form["action"];
        if (action == "save")
        {
            var tasksToUpdate = await _context.EventTasks
            .Where(et => et.EventId == EventId)
            .ToListAsync();

            foreach (var task in tasksToUpdate)
            {
                task.StatusId = doneTaskIds.Contains(task.Id) ? 3 : 1;
                var index = CostTaskIds.IndexOf(task.Id);
                if (index >= 0 && index < CostValues.Count)
                {
                    task.Cost = CostValues[index];
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToPage(new { EventId = this.EventId, SelectedCategory = this.SelectedCategory });

        }
        else if (action.ToString().StartsWith("assign_"))
        {
            var taskIdStr = action.ToString().Replace("assign_", "");
            if (int.TryParse(taskIdStr, out int taskId))
            {
                var task = await _context.EventTasks
                    .FirstOrDefaultAsync(et => et.Id == taskId && et.EventId == EventId);

                if (task != null)
                {
                    var userId = Request.Form["UserId_" + taskId].ToString();
                    task.UserId = string.IsNullOrEmpty(userId) ? null : userId;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { EventId = this.EventId, SelectedCategory = this.SelectedCategory });

        }
        return RedirectToPage(new { EventId = this.EventId, SelectedCategory = this.SelectedCategory });

    }
    
    public async Task<IActionResult> OnPostUploadPhotoAsync(IFormFile photoFile, int eventTaskId, string? description, bool isPublic, int EventId, string? SelectedCategory)
    {
        this.EventId = EventId;
        this.SelectedCategory = SelectedCategory;

        if (photoFile == null || photoFile.Length == 0)
        {
            TempData["StatusMessage"] = "⚠️ Fișierul este invalid.";
            await OnGetAsync(); // ca să reîncarci pagina corect
            return Page();
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(stream);
        }

        var photo = new PhotoGallery
        {
            EventTaskId = eventTaskId,
            Image = "/images/" + fileName,
            Description = description,
            IsPublic = isPublic,
            UserId = _userManager.GetUserId(User),
        };

        _context.PhotoGallery.Add(photo);
        await _context.SaveChangesAsync();

        // 🔄 Reîncarcă tot conținutul paginii ca să fie vizibile și noile poze
        await OnGetAsync();

        TempData["StatusMessage"] = "✅ Poză încărcată cu succes!";
        return Page(); // NU folosi RedirectToPage aici!
    }

}
