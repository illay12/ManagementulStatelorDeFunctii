using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementulStatelorDeFunctii.ViewModels
{
    public class DisciplinaActivitateViewModel
    {
        public string DenumireDisciplina { get; set; }
        public int ActivitateTeoretica{ get; set; } 
        public int ActivitatePractica { get; set; }
        public string ProgramDeStudiu { get; set; }
        public int AnStudiu { get; set; }
        public int NumarGrupe { get; set; }
        public int Semestru { get; set; }
    }
}
