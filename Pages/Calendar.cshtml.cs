using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using RazorPagesEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesEvents.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly EventDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CalendarModel(EventDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<object> Events { get; set; } = new();
        public List<Event> UpcomingEvents { get; set; } = new();

        public async System.Threading.Tasks.Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            // Evenimente create de user SAU la care are acces prin EventUserAccess
            var eventsQuery = _context.Events
                .Where(e =>
                    e.UserId == userId || // creatorul evenimentului
                    e.UserAccesses.Any(a => a.UserId == userId)) // are acces la eveniment
                .Where(e => e.StartDate.HasValue);

            // Calendar
            Events = await eventsQuery
                .Select(e => new
                {
                    title = e.Description ?? "Fără titlu",
                    start = e.StartDate!.Value.ToString("yyyy-MM-dd"),
                    allDay = true
                })
                .ToListAsync<object>();

            // Lista Upcoming Events din sidebar
            UpcomingEvents = await eventsQuery
                .Where(e => e.StartDate >= DateTime.Today)
                .OrderBy(e => e.StartDate)
                .Take(5)
                .ToListAsync();
        }
    }
}
