using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class CadruDidacticGradDidactic
{
    public int CadruDidacticGradDidacticId { get; set; }

    public int CadruDidacticId { get; set; }

    public int GradDidacticId { get; set; }

    public int DeLa { get; set; }

    public int? PanaLa { get; set; }

    public virtual CadruDidactic CadruDidactic { get; set; } = null!;

    public virtual ICollection<CadruDidacticStatFunctieTarifPlataOra> CadruDidacticStatFunctieTarifPlataOras { get; set; } = new List<CadruDidacticStatFunctieTarifPlataOra>();

    public virtual ICollection<DisciplinaCadruDidactic> DisciplinaCadruDidactics { get; set; } = new List<DisciplinaCadruDidactic>();

    public virtual GradDidactic GradDidactic { get; set; } = null!;

    public virtual ICollection<Norma> Normas { get; set; } = new List<Norma>();
}
