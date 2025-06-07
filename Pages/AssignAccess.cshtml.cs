using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesEvents.Models;
using RazorPagesEvents.Services;

public class AssignAccessModel : PageModel
{
    private readonly EventDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    private readonly EmailSender _email;

    public AssignAccessModel(EventDbContext context, UserManager<IdentityUser> userManager, EmailSender email)
    {
        _context = context;
        _userManager = userManager;
        _email = email;
    }

    public List<Event> MyEvents { get; set; } = new();

    [BindProperty]
    public int SelectedEventId { get; set; }

    [BindProperty]
    public string SelectedRole { get; set; } = "Asistent";

    [BindProperty]
    public string EmailToAssign { get; set; } = string.Empty;

    [TempData]
    public string? StatusMessage { get; set; }
    public List<EventUserAccess> AccessList { get; set; } = new();

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        MyEvents = await _context.Events
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();
        if (SelectedEventId > 0)
        {
            AccessList = await _context.UserAccesses
                .Include(a => a.User)
                .Where(a => a.EventId == SelectedEventId)
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = _userManager.GetUserId(User);

        // Verifică dacă e creatorul evenimentului
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == SelectedEventId && e.UserId == userId);
        if (ev == null)
        {
            StatusMessage = "Nu ai dreptul sa modifici acest eveniment.";
            return RedirectToPage(new { SelectedEventId });
        }

        // Caută utilizatorul după email
        var userToAssign = await _userManager.FindByEmailAsync(EmailToAssign);
        if (userToAssign == null)
        {
            var registerUrl = Url.Page("/Account/Register", null, new { area = "Identity" }, Request.Scheme);
            var subject = $"Ai fost invitat(a) sa colaborezi la un eveniment";
            var body = $@"
                <div style='font-family:sans-serif; padding:20px;'>
                    <h2 style='color:#4A90E2;'>👋 Invitatie la eveniment</h2>
                    <p>Buna!</p>
                    <p>Organizatorul evenimentului <strong>{ev.Description}</strong> doreste sa iti ofere rolul <strong>{SelectedRole}</strong>.</p>
                    <p>Pentru a accepta invitatia, creeaza-ti un cont:</p>
                    <p style='text-align:center;'>
                        <a href='{registerUrl}' style='padding:12px 24px; background-color:#4A90E2; color:white; text-decoration:none; border-radius:6px;'>Creeaza cont</a>
                    </p>
                    <p style='margin-top:40px;'>Multumim!</p>
                </div>";
            await _email.SendEmailAsync(EmailToAssign, subject, body);

            StatusMessage = "Utilizatorul nu are cont, dar a primit un email cu link de inregistrare.";
            return RedirectToPage(new { SelectedEventId });
        }

        var acceptUrl = Url.Page("/AcceptInvitation", null, new
            {
                eventId = SelectedEventId,
                userId = userToAssign.Id,
                role = SelectedRole
            }, Request.Scheme);
        
            var subject2 = $"Confirma participarea la eveniment: {ev.Description}";
            var body2 = $@"
                <div style='font-family:sans-serif; padding:20px;'>
                    <h2 style='color:#27ae60;'>✅ Invitatie pentru rolul {SelectedRole}</h2>
                    <p>Buna!</p>
                    <p>Ai fost invitat(a) sa colaborezi la evenimentul <strong>{ev.Description}</strong>.</p>
                    <p>Pentru a accepta, apasa butonul de mai jos:</p>
                    <p style='text-align:center;'>
                        <a href='{acceptUrl}' style='padding:12px 24px; background-color:#27ae60; color:white; text-decoration:none; border-radius:6px;'>Accepta invitatia</a>
                    </p>
                </div>";
        
            await _email.SendEmailAsync(EmailToAssign, subject2, body2);
        
            StatusMessage = $"Email de invitatie trimis. Rolul va fi activ doar dupa ce utilizatorul accepta.";
            return RedirectToPage(new { SelectedEventId });
    
    }

    public async Task<IActionResult> OnPostDeleteAccessAsync(int id)
    {
        var access = await _context.UserAccesses
        .Include(e => e.Event)
        .FirstOrDefaultAsync(ua => ua.Id == id);

        var currentUserId = _userManager.GetUserId(User);

        if (access != null && access.Event.UserId == currentUserId)
        {
            _context.UserAccesses.Remove(access);
            await _context.SaveChangesAsync();
            StatusMessage = "✅ Accesul a fost revocat.";
        }
        return RedirectToPage(new { SelectedEventId = access?.EventId });
    }


}
