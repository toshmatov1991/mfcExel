﻿using System;
using System.Collections.Generic;

namespace exel_for_mfc;

public partial class Area
{
    public int Id { get; set; }

    public string? AreaName { get; set; }

    public bool? AreaBool { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
}
