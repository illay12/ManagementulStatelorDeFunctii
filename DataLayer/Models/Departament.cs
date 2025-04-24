using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class Departament
{
    public int DepartamentId { get; set; }

    public string DenumireDepartament { get; set; } = null!;

    public string Acronim { get; set; } = null!;

    public int FacultateId { get; set; }

    public virtual ICollection<CadruDidactic> CadruDidactics { get; set; } = new List<CadruDidactic>();

    public virtual Facultate Facultate { get; set; } = null!;

    public virtual ICollection<StatDeFunctie> StatDeFuncties { get; set; } = new List<StatDeFunctie>();

    public virtual ICollection<CadruDidactic> CadruDidacticsNavigation { get; set; } = new List<CadruDidactic>();
}
