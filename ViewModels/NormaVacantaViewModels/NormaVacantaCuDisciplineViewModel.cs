using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels
{
    public class NormaVacantaCuDisciplineViewModel
    {
        public int NormaId { get; set; }
        public List<DisciplinaActivitateViewModel> DisciplineActivitati { get; set; } = new();
    }
}