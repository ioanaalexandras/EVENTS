using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesEvents.Pages
{
    public class AcceptInvitationModel : PageModel
    {
        private readonly EventDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AcceptInvitationModel(EventDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Role { get; set; } = "Asistent";

        public string Message { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            // Verifică dacă utilizatorul există
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                Message = "❌ Utilizatorul nu exista.";
                return Page();
            }

            // Verifică dacă evenimentul există
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == EventId);
            if (ev == null)
            {
                Message = "❌ Evenimentul nu a fost gasit.";
                return Page();
            }

            // Verifică dacă deja a fost adăugat
            var alreadyExists = await _context.UserAccesses
                .AnyAsync(a => a.EventId == EventId && a.UserId == UserId);
            if (alreadyExists)
            {
                Message = "✅ Ai acceptat deja invitatia.";
                return Page();
            }

            // Adaugă accesul
            _context.UserAccesses.Add(new EventUserAccess
            {
                EventId = EventId,
                UserId = UserId,
                Role = Role
            });
            await _context.SaveChangesAsync();

            Message = $"✅ Ai fost adaugat ca <strong>{Role}</strong> la evenimentul <strong>{ev.Description}</strong>.";
            return Page();
        }
    }
}
