using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class NormaDisciplina
{
    public int NormaDisciplinaId { get; set; }

    public int NormaId { get; set; }

    public int DisciplinaId { get; set; }

    public int NumarSerii { get; set; }

    public int NumarGrupe { get; set; }

    public bool ActivitateTeoretica { get; set; }

    public bool ActivitatePractica { get; set; }

    public virtual Disciplina Disciplina { get; set; } = null!;

    public virtual ICollection<DisciplinaCadruDidactic> DisciplinaCadruDidactics { get; set; } = new List<DisciplinaCadruDidactic>();

    public virtual Norma Norma { get; set; } = null!;
}
