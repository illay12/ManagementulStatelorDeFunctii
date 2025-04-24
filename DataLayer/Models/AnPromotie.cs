using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class AnPromotie
{
    public int AnPromotieId { get; set; }

    public int NumarSerii { get; set; }

    public int NumărGrupe { get; set; }

    public int PromotieId { get; set; }

    public int AnDeStudiu { get; set; }

    public int NumarSaptamaniActivitate { get; set; }

    public virtual ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();

    public virtual Promotie Promotie { get; set; } = null!;
}
