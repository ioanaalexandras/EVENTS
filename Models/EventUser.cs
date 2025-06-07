using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models;

public partial class EventUser
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = default!;

    public virtual IdentityUser User { get; set; } = default!;
}
