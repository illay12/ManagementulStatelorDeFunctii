using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementulStatelorDeFunctii.DataLayer.Repos
{
    public class CadruDidacticStatFunctieTarifPlataOraRepository 
        : GenericRepository<CadruDidacticStatFunctieTarifPlataOra>, ICadruDidacticStatFunctieTarifPlataOraRepository
    {
        private readonly StateDeFunctieDbbContext _context;

        public CadruDidacticStatFunctieTarifPlataOraRepository(StateDeFunctieDbbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistaAsociereAsync(int statFunctieId, int cadruDidacticId)
        {
            return await _context.CadruDidacticStatFunctieTarifPlataOras
                .Include(t => t.CadruDidactic)
                .AnyAsync(t =>
                    t.StatFunctieId == statFunctieId &&
                    t.CadruDidactic.CadruDidacticId == cadruDidacticId);
        }

    }

}