using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.ViewModels
{
    public class CadruDidacticFormViewModel
    {
        public CadruDidactic CadruDidactic { get; set; }
        public IEnumerable<SelectListItem>? GradeDisponibile { get; set; }
        public IEnumerable<SelectListItem>? DepartamenteDisponibile { get; set; }
        
    }
}