using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces
{
    public interface INormaRepository
    {
        Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId,int statDeFunctieId);
        Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int statDeFunctieId,int cadruDidacticId);
        Task AsociazaDisciplinaLaCadruAsync(
            int cadruDidacticId,
            int disciplinaId,
            int numarSerii,
            int numarGrupe,
            bool areActivitateTeoretica,
            bool areActivitatePractica);
        Task EliminaDisciplinaDeLaCadruAsync(int cadruDidacticId, int normaDisciplinaId);
        Task<Norma> GetNormaByCadruDidacticIdAsync(int cadruDidacticId,int statDeFunctieId);
        Task<NormaDisciplina> GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(int cadruDidacticId, int disciplinaId);
        Task<IActionResult> EliminaAsociereDisciplinaDeLaCadruAsync(int normaDisciplinaId);
        Task<IActionResult> CreazaNormaAsync(int cadruDidacticId, int statDeFunctieId);
    
    }

}