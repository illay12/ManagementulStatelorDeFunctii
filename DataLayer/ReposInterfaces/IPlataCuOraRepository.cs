using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces
{
    public interface IPlataCuOraRepository
    {
        public Task<List<CadruDidacticCuDisciplinePlataCuOraViewModel>> GetProfesoriCuDisciplineAsync(int statId, int semestru);
        public Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int cadruDidacticId, int statDeFunctieId, int semestru);
        public Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId, int statDeFunctieId, int semestru);
        public Task<DisciplinaCadruDidactic> GetDisciplinaCadruDidacticByNormaDiscIdCadruId(int NormaDisciplinaId, int cadruDidacticId);
        public Task<IEnumerable<CadruDidacticGradDidactic>> GetCadreDidacticeDisponibileLaAdaugare(int statDeFunctieId, int semestru);
        public Task<int> GetCadruDidacticGradDidacticIDLaPlataCuOraByCadruDidacticIdAsync(int cadruDidacticId);
        public Task<int> GetGrupeDisponibileDinNormaVacantaAsync(int disciplinaId, int statDeFunctieId);

    }
}