using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesEvents.Models;

public class MyEventsModel : PageModel
{
    private readonly EventDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public MyEventsModel(EventDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public List<ProgressEvent> UserEvents { get; set; } = new();
    public List<AccessEvent> EventsIAccess { get; set; } = new();
    public List<EventUserAccess> EventAccessList { get; set; } = new();

    public class ProgressEvent
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? EventTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public int Progress { get; set; }
    }

    public class AccessEvent
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? EventTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public int Progress { get; set; }
        public string Role { get; set; } = null!;
    }
    [BindProperty]
    public int AccessId { get; set; }

    [BindProperty]
    public int DeleteEvent { get; set; }
    [BindProperty(SupportsGet = true)]
    public int? ShowEditForm { get; set; }
    [BindProperty]
    public int EditEvent { get; set; }
    [BindProperty]
    public string? EditDescription { get; set; }
    [BindProperty]
    public DateTime? EditStartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FilterType { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDateFrom { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDateTo { get; set; }
    public List<string> AllEventTypes { get; set; } = new();

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        var createdEvents = await _context.Events
            .Include(e => e.EventType)
            .Include(e => e.EventTasks)
                .ThenInclude(t => t.Status)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();
        AllEventTypes = await _context.EventTypes
            .OrderBy(et => et.Name)
            .Select(et => et.Name)
            .Distinct()
            .ToListAsync();

        if (!string.IsNullOrEmpty(FilterType))
        {
            createdEvents = createdEvents.Where(e => e.EventType?.Name == FilterType).ToList();
        }
        if (StartDateFrom.HasValue)
        {
            createdEvents = createdEvents.Where(e => e.StartDate >= StartDateFrom).ToList();
        }

        if (StartDateTo.HasValue)
        {
            createdEvents = createdEvents.Where(e => e.StartDate <= StartDateTo).ToList();
        }

        UserEvents = createdEvents.Select(e =>
        {
            var totalTasks = e.EventTasks.Count;
            var completedTasks = e.EventTasks.Count(t => t.Status != null && t.Status.Name == "Done");
            var progress = totalTasks > 0 ? (int)Math.Round((double)(completedTasks * 100) / totalTasks) : 0;

            return new ProgressEvent
            {
                Id = e.Id,
                Description = e.Description,
                EventTypeName = e.EventType?.Name,
                StartDate = e.StartDate,
                Progress = progress
            };
        }).ToList();

        var accessList = await _context.UserAccesses
           .Include(a => a.Event).ThenInclude(e => e.EventType)
           .Include(a => a.Event).ThenInclude(e => e.EventTasks)
               .ThenInclude(t => t.Status)
           .Where(a => a.UserId == userId)
           .ToListAsync();

        EventsIAccess = accessList.Select(a =>
        {
            var myTasks = a.Event.EventTasks.Where(t => t.UserId == userId).ToList();
            var total = myTasks.Count;
            var done = myTasks.Count(t => t.Status?.Name == "Done");
            var progress = total > 0 ? (int)Math.Round((double)(done * 100) / total) : 0;

            return new AccessEvent
            {
                Id = a.Event.Id,
                Description = a.Event.Description,
                EventTypeName = a.Event.EventType?.Name,
                StartDate = a.Event.StartDate,
                Progress = progress,
                Role = a.Role
            };
        }).ToList();

        EventAccessList = await _context.UserAccesses
            .Include(a => a.Event)
            .Include(a => a.User)
            .Where(a => a.Event.UserId == userId)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        var ev = await _context.Events.FindAsync(EditEvent);
        if (ev != null)
        {
            ev.Description = EditDescription;
            ev.StartDate = EditStartDate;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var ev = await _context.Events
            .Include(e => e.EventTasks)
            .FirstOrDefaultAsync(e => e.Id == DeleteEvent);

        if (ev != null)
        {
            // 1. Ștergi mai întâi taskurile
            _context.EventTasks.RemoveRange(ev.EventTasks);

            // 2. Apoi ștergi evenimentul
            _context.Events.Remove(ev);

            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRevokeAccessAsync()
    {
        var access = await _context.UserAccesses.FindAsync(AccessId);
        if (access != null)
        {
            _context.UserAccesses.Remove(access);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

}