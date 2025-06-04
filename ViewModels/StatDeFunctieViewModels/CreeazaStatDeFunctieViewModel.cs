using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.ViewModels.StatDeFunctieViewModels
{
    public class CreeazaStatDeFunctieViewModel
    {
        public int DepartamentId { get; set; }
        public int AnStat { get; set; }
        public List<SelectListItem> Departamente { get; set; }
    }
}