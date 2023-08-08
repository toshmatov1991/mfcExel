using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class CertificateSolution
{
    public int Id { get; set; }

    public DateTime? DateDecision { get; set; }

    public string? NumberDecision { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
