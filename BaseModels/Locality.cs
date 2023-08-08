using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Locality
{
    public int Id { get; set; }

    public string? LocalName { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
