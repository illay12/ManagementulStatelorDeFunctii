using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class GradDidactic
{
    public int GradDidacticId { get; set; }

    public string NumeGrad { get; set; } = null!;

    public int Prioritate { get; set; }

    public int? GradDidacticCategorieId { get; set; }

    public virtual ICollection<CadruDidacticGradDidactic> CadruDidacticGradDidactics { get; set; } = new List<CadruDidacticGradDidactic>();

    public virtual ICollection<CadruDidactic> CadruDidactics { get; set; } = new List<CadruDidactic>();

    public virtual GradDidacticNormabil? GradDidacticNormabil { get; set; }
}
