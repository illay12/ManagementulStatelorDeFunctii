using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces
{
    public interface INormaRepository : IGenericRepository<Norma>
    {
        Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId, int statDeFunctieId);
        Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int statDeFunctieId, int cadruDidacticId);
        Task AsociazaDisciplinaLaCadruAsync(
            int cadruDidacticId,
            int disciplinaId,
            int numarSerii,
            int numarGrupe,
            bool areActivitateTeoretica,
            bool areActivitatePractica);
        Task EliminaDisciplinaDeLaCadruAsync(int cadruDidacticId, int normaDisciplinaId);
        Task<Norma> GetNormaByCadruDidacticIdAsync(int cadruDidacticId, int statDeFunctieId);
        Task<NormaDisciplina> GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(int cadruDidacticId, int disciplinaId);
        Task<IActionResult> EliminaAsociereDisciplinaDeLaCadruAsync(int normaDisciplinaId);
        Task<IActionResult> CreazaNormaAsync(int cadruDidacticId, int statDeFunctieId);
        Task<NormaDisciplina> GetNormaDisciplinaVacantaByDisciplinaIdAsync(int disciplinaId, int statDeFunctieId);
        Task<NormaDisciplina> GetNormaDisciplinaAsociataCadruAsync(int cadruDidacticId, int disciplinaId, int statDeFunctieId);
        Task<List<NormaVacantaCuDisciplineViewModel>> GetNormeVacanteCuDisciplineAsync(int statDeFunctieId);
        Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateLaNormaVacantaAsync(int normaId);
        Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileLaNormaVacantaAsync(int normaId);
        Task<int> GetNumarGrupeOcupatePentruDisciplinaAsync(int disciplinaId, int statDeFunctieId);
        Task<int> GetNumarGrupeTotal(int disciplinaId, int statDeFunctieId);
    }

}