using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Privilege
{
    public int Id { get; set; }

    public string? PrivilegesName { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
