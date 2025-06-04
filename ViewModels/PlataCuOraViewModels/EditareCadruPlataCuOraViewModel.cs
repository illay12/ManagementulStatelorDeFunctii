using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels
{
    public class EditareCadruPlataCuOraViewModel
    {
        public int CadruDidacticId { get; set; }
        public string NumeProfesor { get; set; }
        public int Semestru { get; set; }
        public int StatDeFunctieId { get; set; }
        public List<DisciplinaNormaViewModel> DisciplineAsociate { get; set; }
        public List<SelectListItem> DisciplineDisponibile { get; set; }

    }
}