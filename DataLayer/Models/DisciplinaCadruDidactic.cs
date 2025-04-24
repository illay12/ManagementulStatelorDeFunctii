using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class DisciplinaCadruDidactic
{
    public int NormaDisciplinaCadruDidacticId { get; set; }

    public int NormaDisciplinaId { get; set; }

    public int CadruDidacticId { get; set; }

    public virtual CadruDidacticGradDidactic CadruDidactic { get; set; } = null!;

    public virtual NormaDisciplina NormaDisciplina { get; set; } = null!;
}
