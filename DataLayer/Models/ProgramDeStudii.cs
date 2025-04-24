using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class ProgramDeStudii
{
    public int ProgramStudiiId { get; set; }

    public int FacultateId { get; set; }

    public string Nume { get; set; } = null!;

    public string? Acronim { get; set; }

    public decimal CoeficientKActivitateTeoretica { get; set; }

    public int TipProgramId { get; set; }

    public string LimbaStudiu { get; set; } = null!;

    public decimal CoeficientKActivitatePractica { get; set; }

    public virtual Facultate Facultate { get; set; } = null!;

    public virtual ICollection<Promotie> Promoties { get; set; } = new List<Promotie>();

    public virtual TipProgramStudiu TipProgram { get; set; } = null!;
}
