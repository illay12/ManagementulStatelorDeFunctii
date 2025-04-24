using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class Norma
{
    public int NormaId { get; set; }

    public int StatDefunctieId { get; set; }

    public int GradDidacticId { get; set; }

    public bool Vacant { get; set; }

    public virtual GradDidacticNormabil GradDidactic { get; set; } = null!;

    public virtual ICollection<NormaDisciplina> NormaDisciplinas { get; set; } = new List<NormaDisciplina>();

    public virtual StatDeFunctie StatDefunctie { get; set; } = null!;

    public virtual ICollection<CadruDidacticGradDidactic> CadruDidactics { get; set; } = new List<CadruDidacticGradDidactic>();
}
