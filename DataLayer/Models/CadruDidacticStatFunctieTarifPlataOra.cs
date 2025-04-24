using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class CadruDidacticStatFunctieTarifPlataOra
{
    public int CadruDidacticStatFunctieTarifId { get; set; }

    public int CadruDidacticId { get; set; }

    public int StatFunctieId { get; set; }

    public int Tarif { get; set; }

    public virtual CadruDidacticGradDidactic CadruDidactic { get; set; } = null!;

    public virtual StatDeFunctie StatFunctie { get; set; } = null!;
}
