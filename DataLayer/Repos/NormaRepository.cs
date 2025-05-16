using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementulStatelorDeFunctii.DataLayer.Repos
{ 
    public class NormaRepository : INormaRepository
    {
        private readonly StateDeFunctieDbbContext _context;

        public NormaRepository(StateDeFunctieDbbContext context)
        {
            _context = context;
        }

        public async Task AsociazaDisciplinaLaCadruAsync(
            int cadruDidacticId,
            int disciplinaId,
            int numarSerii,
            int numarGrupe,
            bool areActivitateTeoretica,
            bool areActivitatePractica)
        {
            var norma = await _context.Normas
                .Include(n => n.CadruDidactics)
                .FirstOrDefaultAsync(n => n.CadruDidactics.Any(cd => cd.CadruDidacticId == cadruDidacticId));

            if (norma == null)
            {
                throw new InvalidOperationException("Cadru didactic nu are o normă asociată.");
            }

            var asociere = new NormaDisciplina
            {
                NormaId = norma.NormaId,
                DisciplinaId = disciplinaId,
                NumarSerii = numarSerii,
                NumarGrupe = numarGrupe,
                ActivitateTeoretica = areActivitateTeoretica,
                ActivitatePractica = areActivitatePractica
            };

            _context.NormaDisciplinas.Add(asociere);
            await _context.SaveChangesAsync();
        }



        // pentru plata cu ora momentan nu il folosesc
        public async Task EliminaDisciplinaDeLaCadruAsync(int cadruDidacticId, int normaDisciplinaId)
        {
            var disciplina = await _context.DisciplinaCadruDidactics
                .FirstOrDefaultAsync(d => d.CadruDidacticId == cadruDidacticId && d.NormaDisciplinaId == normaDisciplinaId);

            if (disciplina != null)
            {
                _context.DisciplinaCadruDidactics.Remove(disciplina);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId,int statDeFunctieId)
        {
            return await _context.Normas
                .Where(n => n.CadruDidactics.Any(cdgd => cdgd.CadruDidacticId == cadruDidacticId) && n.StatDefunctieId == statDeFunctieId)
                .SelectMany(n => n.NormaDisciplinas)
                .Include(nd => nd.Disciplina)
                .Select(nd => new DisciplinaNormaViewModel
                {
                    DisciplinaId = nd.DisciplinaId,
                    NormaDisciplinaId = nd.NormaDisciplinaId,
                    Denumire = nd.Disciplina.DenumireDisciplina
                })
                .ToListAsync();

        }

        //aici e de schimbat
        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int statDeFunctieId,int cadruDidacticId)
        {
            int anStat = await _context.StatDeFuncties
                .Where(s => s.StatDeFunctieId == statDeFunctieId)
                .Select(s => s.An)
                .FirstOrDefaultAsync();

            System.Console.WriteLine(anStat);
            var asociateIds = await _context.NormaDisciplinas
                .Where(n => n.Norma.StatDefunctieId == statDeFunctieId && n.Norma.CadruDidactics.Any(cd => cd.CadruDidacticId == cadruDidacticId))
                .Select(d => d.DisciplinaId)
                .Distinct()
                .ToListAsync();

            var disponibile = await _context.Disciplinas
                .Where(d =>
                    !asociateIds.Contains(d.DisciplinaId) &&
                    d.AnPromotie.Promotie.AnInceput + (d.AnPromotie.AnDeStudiu - 1) == anStat
                )
                .Select(d => new DisciplinaNormaViewModel
                {
                    NormaDisciplinaId = 0,
                    DisciplinaId = d.DisciplinaId,
                    Denumire = d.DenumireDisciplina + " - " +
                            d.AnPromotie.Promotie.ProgramDeStudii.Acronim + " " +
                            d.AnPromotie.AnDeStudiu + " " + d.DisciplinaId + " " + d.AnPromotie.Promotie.AnInceput + " " + d.AnPromotie.AnDeStudiu
                })
                .OrderBy(d => d.Denumire)
                .ToListAsync();

            return disponibile;

        }

        public async Task<Norma> GetNormaByCadruDidacticIdAsync(int cadruDidacticId,int statDeFunctieId)
        {
            return await _context.Normas
                .Include(n => n.CadruDidactics)
                .FirstOrDefaultAsync(n=> n.CadruDidactics.Any(cd => cd.CadruDidacticId == cadruDidacticId) && n.StatDefunctieId == statDeFunctieId)
                ?? throw new InvalidOperationException("Norma nu a fost gasita!");

        }

        public async Task<NormaDisciplina> GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(int cadruDidacticId, int disciplinaId)
        {
            return await _context.NormaDisciplinas
                .Include(nd => nd.Norma)
                .FirstOrDefaultAsync(nd => nd.DisciplinaId == disciplinaId && nd.Norma.CadruDidactics.Any(cd => cd.CadruDidacticId == cadruDidacticId))
                ?? throw new InvalidOperationException("NormaDisciplina nu a fost gasita!");
        }
        
        public async Task<IActionResult> EliminaAsociereDisciplinaDeLaCadruAsync(int normaDisciplinaId)
        {
            var disciplina = await _context.NormaDisciplinas
                .FirstOrDefaultAsync(d => d.NormaDisciplinaId == normaDisciplinaId);

            if (disciplina != null)
            {
                _context.NormaDisciplinas.Remove(disciplina);
                await _context.SaveChangesAsync();
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        
        
        }

        public async Task<IActionResult> CreazaNormaAsync(int cadruDidacticId,int statDeFunctieId)
        {
            var norma = new Norma{
                StatDefunctieId = statDeFunctieId,
                GradDidacticId = 1,
                Vacant = false
            };
            
            var cadru = await _context.CadruDidacticGradDidactics
                .Where(cdgd => cdgd.PanaLa == null)
                .FirstOrDefaultAsync(cdgd => cdgd.CadruDidacticId == cadruDidacticId);

            if (cadru == null)
            {
                return new BadRequestObjectResult("Nu s-a găsit niciun grad activ pentru acest cadru didactic.");
            }
            _context.Attach(cadru);
            norma.CadruDidactics.Add(cadru);
            await _context.Normas.AddAsync(norma);

            
            return await _context.SaveChangesAsync() > 0 
                ? new OkResult()
                : new BadRequestResult();
        }
    }

}