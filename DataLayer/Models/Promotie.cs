using System;
using System.Collections.Generic;

namespace ManagementulStatelorDeFunctii.Models;

public partial class Promotie
{
    public int PromotieId { get; set; }

    public int ProgramDeStudiiId { get; set; }

    public int AnInceput { get; set; }

    public int NumarAni { get; set; }

    public virtual ICollection<AnPromotie> AnPromoties { get; set; } = new List<AnPromotie>();

    public virtual ProgramDeStudii ProgramDeStudii { get; set; } = null!;
}
