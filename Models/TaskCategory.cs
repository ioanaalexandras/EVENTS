using System;
using System.Collections.Generic;

namespace RazorPagesEvents.Models;

public partial class TaskCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
