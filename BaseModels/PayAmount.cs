using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class PayAmount
{
    public int Id { get; set; }

    public decimal? Pay { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
