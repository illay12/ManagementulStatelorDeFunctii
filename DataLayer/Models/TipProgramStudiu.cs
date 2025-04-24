using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class TipProgramStudiu
{
    public int TipProgramStudiuId { get; set; }

    public string DenumireTipProgram { get; set; } = null!;

    public virtual ICollection<ProgramDeStudii> ProgramDeStudiis { get; set; } = new List<ProgramDeStudii>();
}
