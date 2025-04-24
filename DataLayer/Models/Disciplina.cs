using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class Disciplina
{
    public int DisciplinaId { get; set; }

    public string DenumireDisciplina { get; set; } = null!;

    public int AnPromotieId { get; set; }

    public int ActivitateTeoretica { get; set; }

    public int ActivitatePractica { get; set; }

    public int SemestruDeDesfasurare { get; set; }

    public int NumarSaptamani { get; set; }

    public bool Ruleaza { get; set; }

    public string? Acronim { get; set; }

    public virtual AnPromotie AnPromotie { get; set; } = null!;

    public virtual ICollection<NormaDisciplina> NormaDisciplinas { get; set; } = new List<NormaDisciplina>();
}
