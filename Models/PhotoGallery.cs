using System;
using System.Collections.Generic;

namespace RazorPagesEvents.Models;

public partial class PhotoGallery
{
    public Guid Id { get; set; }

    public int EventTaskId { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public virtual EventTask EventTask { get; set; } = null!;
}
