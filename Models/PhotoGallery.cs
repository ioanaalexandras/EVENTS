using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models;

public partial class PhotoGallery
{
    public Guid Id { get; set; }

    public string? UserId { get; set; }

    public virtual IdentityUser? User { get; set; }

    public int EventTaskId { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public virtual EventTask EventTask { get; set; } = null!;
    [NotMapped]
    public string? EventType { get; set; }
    [NotMapped]
    public string? Category { get; set; }

}
