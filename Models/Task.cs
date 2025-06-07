using System;
using System.Collections.Generic;

namespace RazorPagesEvents.Models;

public partial class Task
{
    public int Id { get; set; }

    public int EventTypeId { get; set; }

    public int TaskCategoryId { get; set; }

    public string? Description { get; set; }

    public int? OrderNo { get; set; }

    public decimal? Cost { get; set; }

    public virtual ICollection<EventTask> EventTasks { get; set; } = new List<EventTask>();

    public virtual EventType EventType { get; set; } = null!;

    public virtual TaskCategory TaskCategory { get; set; } = null!;
}
