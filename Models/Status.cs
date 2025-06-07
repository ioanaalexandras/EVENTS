using System;
using System.Collections.Generic;

namespace RazorPagesEvents.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<EventTask> EventTasks { get; set; } = new List<EventTask>();
}
