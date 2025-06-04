using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels
{
    public class AsociereDisciplinaNormaVacantaViewModel
    {
        public int NormaId { get; set; }
        public int DisciplinaId { get; set; }
        [Range(1, 2, ErrorMessage = "Numărul de serii trebuie să fie cel puțin 1.")]
        public int NumarSerii { get; set; }
        public int NumarGrupe { get; set; }
        public int StatDeFunctieId { get; set; }

    }
}