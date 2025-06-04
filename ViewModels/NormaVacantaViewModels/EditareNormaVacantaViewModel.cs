using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels
{
    public class EditareNormaVacantaViewModel
    {
        public int NormaId { get; set; }
        public int StatDeFunctieId { get; set; }
        public List<DisciplinaNormaViewModel> DisciplineAsociate { get; set; } = new();
        public List<SelectListItem> DisciplineDisponibile { get; set; } = new();
    }
}