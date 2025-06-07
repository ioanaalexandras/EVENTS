using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesEvents.Models;
using System.Threading.Tasks;
public class ChooseEventModel : PageModel
{
    private readonly EventDbContext _context;

    private readonly UserManager<IdentityUser> _userManager;

    public ChooseEventModel(EventDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public int SelectedEventTypeId { get; set; }

    [BindProperty]
    public string? EventName { get; set; }

    public List<SelectListItem>? EventTypes { get; set; }

    [BindProperty]
    public int SelecteazaMoneda { get; set; } 
    public List<Moneda> Monede { get; set; } = new();

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        EventTypes = await _context.EventTypes
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Name })
            .ToListAsync();

        Monede = await _context.Moneda.ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || SelectedEventTypeId == 0 || string.IsNullOrWhiteSpace(EventName))
        {
            await OnGetAsync(); // Reload dropdown list
            return Page();
        }
        var userId = _userManager.GetUserId(User);

        // Creeaza un eveniment nou
        var newEvent = new Event
        {
            EventTypeId = SelectedEventTypeId,
            Description = EventName,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            UserId = userId,
            MonedaId = SelecteazaMoneda
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        // Obtine task-urile sablon pentru acel EventType
        var sablonTasks = await _context.Tasks
            .Where(t => t.EventTypeId == SelectedEventTypeId)
            .ToListAsync();

        // Creeaza EventTask uri 
        foreach (var sablon in sablonTasks)
        {
            var eventTask = new EventTask
            {
                EventId = newEvent.Id,
                TaskId = sablon.Id,
                UserId = userId,
                StatusId = 1,
                Cost = null
            };

            _context.EventTasks.Add(eventTask);
        }

        await _context.SaveChangesAsync();

        // Redirecționează către pagina de checklist
        return RedirectToPage("Checklist", new { eventId = newEvent.Id });
    }
}
