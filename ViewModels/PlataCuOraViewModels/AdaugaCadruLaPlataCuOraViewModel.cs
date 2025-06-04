using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels
{
    public class AdaugaCadruLaPlataCuOraViewModel
    {
        public int StatDeFunctieId {get; set;}
        public int CadruDidacticId { get; set; }
        public int Semestru { get; set; }
        public List<SelectListItem> CadreDidacticeDisponibile { get; set; } = new List<SelectListItem>();
    }
}