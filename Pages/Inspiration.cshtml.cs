using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesEvents.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace RazorPagesEvents.Pages;

public class InspirationModel : PageModel
{
    private readonly EventDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public InspirationModel(EventDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<PhotoGallery> PublicPhotos { get; set; } = new();
    [BindProperty(SupportsGet = true)]
    public List<Guid> FavoritePhotoIds { get; set; } = new();
    [BindProperty(SupportsGet = true)]
    public bool ShowFavorites {get; set;}

    public List<string> AvailableEventTypes { get; set; } = new();
    public List<string> AvailableCategories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SelectedEventType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedCategory { get; set; }

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        FavoritePhotoIds = await _context.FavoritePhotos
            .Where(f => f.UserId == userId)
            .Select(f => f.PhotoGalleryId)
            .ToListAsync();

        var query = _context.PhotoGallery
            .Include(p => p.User)
            .Include(p => p.EventTask)
                .ThenInclude(et => et.Task)
                    .ThenInclude(t => t.TaskCategory)
            .Include(p => p.EventTask.Event)
                .ThenInclude(e => e.EventType)
            .Where(p => p.IsPublic);

        if (!string.IsNullOrEmpty(SelectedEventType))
        {
            query = query.Where(p =>
            p.EventTask.Event != null &&
            p.EventTask.Event.EventType != null &&
            p.EventTask.Event.EventType.Name == SelectedEventType);
        }

        if (!string.IsNullOrEmpty(SelectedCategory))
        {
            query = query.Where(p => p.EventTask.Task.TaskCategory.Name == SelectedCategory);
        }

        if (ShowFavorites)
        {
            query = query.Where(p => FavoritePhotoIds.Contains(p.Id));
        }

        var photosFromDb = await query.ToListAsync();

        PublicPhotos = photosFromDb.Select(p => new PhotoGallery
        {
            Id = p.Id,
            Image = p.Image,
            Description = p.Description,
            IsPublic = p.IsPublic,
            EventTaskId = p.EventTaskId,
            EventType = p.EventTask.Event.EventType?.Name ?? "",
            Category = p.EventTask.Task.TaskCategory?.Name ?? "",
            User = p.User
        }).ToList();

        // Pentru dropdown-uri
        AvailableEventTypes = await _context.EventTypes
            .Select(et => et.Name)
            .Distinct()
            .ToListAsync();

        AvailableCategories = await _context.TaskCategories
            .Select(c => c.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleFavoriteAsync(Guid photoId)
    {
        var userId = _userManager.GetUserId(User);

        var favorite = await _context.FavoritePhotos
            .FirstOrDefaultAsync(f => f.UserId == userId && f.PhotoGalleryId == photoId);

        if (favorite != null)
        {
            _context.FavoritePhotos.Remove(favorite);
        }
        else
        {
            _context.FavoritePhotos.Add(new FavoritePhoto
            {
                UserId = userId,
                PhotoGalleryId = photoId
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToPage(); // Reîncarcă pagina
    }
}
