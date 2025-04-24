using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class Facultate
{
    public int FacultateId { get; set; }

    public string Nume { get; set; } = null!;

    public string Acronim { get; set; } = null!;

    public bool Intern { get; set; }

    public virtual ICollection<Departament> Departaments { get; set; } = new List<Departament>();

    public virtual ICollection<ProgramDeStudii> ProgramDeStudiis { get; set; } = new List<ProgramDeStudii>();
}
