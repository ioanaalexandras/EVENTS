using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models;

public partial class Event
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    public int EventTypeId { get; set; }

    public int MonedaId { get; set; }
    public virtual Moneda? Moneda { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<EventTask> EventTasks { get; set; } = new List<EventTask>();

    public virtual EventType? EventType { get; set; }

    public virtual ICollection<EventUser> EventUsers { get; set; } = new List<EventUser>();
    public virtual ICollection<EventUserAccess> UserAccesses { get; set; } = new List<EventUserAccess>();


    
}
