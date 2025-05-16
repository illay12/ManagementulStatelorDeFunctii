using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementulStatelorDeFunctii.ViewModels
{
    public class ProfesorDisciplinePeStatViewModel
    {
        public int CadruDidacticId { get; set; }
        public string NumeProfesor { get; set; }
        public string Grad { get; set; }
        public List<DisciplinaActivitateViewModel> DisciplineActivitati { get; set; } = new();
    }

}