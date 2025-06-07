using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models;

public partial class EventTask
{
    public int Id { get; set; }

    public int EventId { get; set; }
    
    public virtual Event Event { get; set; } = null!;

    public string? UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;
   
    public int TaskId { get; set; }
    public virtual Task Task { get; set; } = null!;

    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public decimal? Cost { get; set; }

    public int? StatusId { get; set; }
    public virtual Status? Status { get; set; }

    public virtual ICollection<PhotoGallery> PhotoGalleries { get; set; } = new List<PhotoGallery>();

    
}
