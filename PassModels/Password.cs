using System;
using System.Collections.Generic;

namespace exel_for_mfc.PassModels;

public partial class Password
{
    public long Id { get; set; }

    public string? Login { get; set; }

    public string? Pass { get; set; }
}
