using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class GradDidacticNormabil
{
    public int GradDidacticNormabilId { get; set; }

    public bool Normabil { get; set; }

    public virtual GradDidactic GradDidacticNormabilNavigation { get; set; } = null!;

    public virtual ICollection<Norma> Normas { get; set; } = new List<Norma>();
}
