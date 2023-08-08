using System;
using System.Collections.Generic;

namespace exel_for_mfc.Models;

public partial class Adress
{
    public int Id { get; set; }

    public string? UlMkr { get; set; }

    public string? NameStreet { get; set; }

    public string? DomOrStr { get; set; }

    public int? IfExistStr { get; set; }

    public int? ApartNumb { get; set; }

    public virtual ICollection<Registry> Registries { get; set; } = new List<Registry>();
}
