using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Fio
{
    public int Id { get; set; }

    public string? Firstname { get; set; }

    public string? Middlename { get; set; }

    public string? Lastname { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
