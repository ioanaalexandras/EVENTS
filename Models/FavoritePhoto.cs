using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models;
public class FavoritePhoto
{
    public int Id { get; set; }

    [Required]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    [Required]
    public Guid PhotoGalleryId { get; set; }

    [ForeignKey("PhotoGalleryId")]
    public virtual PhotoGallery? PhotoGallery { get; set; } = null!;
}

