using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exel_for_mfc;

public partial class Area
{
    public int Id { get; set; }

    public string? AreaName { get; set; }

    public int? HidingArea { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
}
