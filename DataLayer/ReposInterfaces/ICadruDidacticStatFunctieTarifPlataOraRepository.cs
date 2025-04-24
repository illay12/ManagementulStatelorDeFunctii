using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;

namespace ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces
{
    public interface ICadruDidacticStatFunctieTarifPlataOraRepository : IGenericRepository<CadruDidacticStatFunctieTarifPlataOra>
    {
        Task<bool> ExistaAsociereAsync(int statFunctieId, int cadruId);
    }
}
