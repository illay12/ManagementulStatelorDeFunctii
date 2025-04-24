using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class StatDeFunctie
{
    public int StatDeFunctieId { get; set; }

    public int DepartamentId { get; set; }

    public int An { get; set; }

    public bool Draft { get; set; }

    public virtual ICollection<CadruDidacticStatFunctieTarifPlataOra> CadruDidacticStatFunctieTarifPlataOras { get; set; } = new List<CadruDidacticStatFunctieTarifPlataOra>();

    public virtual Departament Departament { get; set; } = null!;

    public virtual ICollection<Norma> Normas { get; set; } = new List<Norma>();
}
