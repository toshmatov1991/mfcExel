using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Area
{
    public int Id { get; set; }

    public string? AreaName { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
