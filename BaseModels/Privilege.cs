using System;
using System.Collections.Generic;

namespace exel_for_mfc;

public partial class Privilege
{
    public int Id { get; set; }

    public string? PrivilegesName { get; set; }

    public bool? PrivBool { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
}
