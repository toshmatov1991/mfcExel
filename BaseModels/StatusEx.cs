using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class StatusEx
{
    public int Id { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
