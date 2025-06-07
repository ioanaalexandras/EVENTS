using System;
using System.Collections.Generic;

namespace RazorPagesEvents.Models;

public partial class Moneda
{
    public int Id { get; set; }

    public string Cod { get; set; } = null!;

    public string Simbol { get; set; } = null!;

    public string Descriere { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
