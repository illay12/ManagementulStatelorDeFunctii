using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class CadruDidactic
{
    public int CadruDidacticId { get; set; }

    public string? Nume { get; set; }

    public int GradDidacticId { get; set; }

    public int DepartamentId { get; set; }

    public virtual ICollection<CadruDidacticGradDidactic> CadruDidacticGradDidactics { get; set; } = new List<CadruDidacticGradDidactic>();

    public virtual Departament? Departament { get; set; }

    public virtual GradDidactic? GradDidactic { get; set; }

    public virtual ICollection<Departament> Departaments { get; set; } = new List<Departament>();
}
