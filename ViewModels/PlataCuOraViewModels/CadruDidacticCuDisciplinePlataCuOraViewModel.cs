using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels
{
    public class CadruDidacticCuDisciplinePlataCuOraViewModel
    {
        public int CadruDidacticId { get; set; }
        public string NumeProfesor { get; set; }
        public string Grad { get; set; }
        public decimal TarifPlataCuOra { get; set; }
        public decimal TotalOrePeSemestru { get; set; }
        public decimal TotalPlataPeSemestru { get; set; }
        public List<DisciplinaActivitateViewModel> DisciplineActivitati { get; set; } = new();
    }
}