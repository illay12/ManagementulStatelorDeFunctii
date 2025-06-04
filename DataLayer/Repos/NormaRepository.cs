using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagementulStatelorDeFunctii.DataLayer.Repos
{
    public class NormaRepository : GenericRepository<Norma>, INormaRepository
    {
        private readonly StateDeFunctieDbbContext _context;

        public NormaRepository(StateDeFunctieDbbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<NormaDisciplina> GetNormaDisciplinaAsociataCadruAsync(int cadruDidacticId, int disciplinaId, int statDeFunctieId)
        {
            return await _context.NormaDisciplinas
                .Include(nd => nd.Norma)
                .FirstOrDefaultAsync(nd => nd.DisciplinaId == disciplinaId
                && nd.DisciplinaCadruDidactics.Any(cd => cd.CadruDidacticId == cadruDidacticId)
                && nd.Norma.StatDefunctieId == statDeFunctieId
                && nd.Norma.Vacant == true)
                ?? throw new InvalidOperationException("Nu a fost gasita!");
        }
        public async Task<NormaDisciplina> GetNormaDisciplinaVacantaByDisciplinaIdAsync(int disciplinaId, int statDeFunctieId)
        {
            return await _context.NormaDisciplinas
                .FirstOrDefaultAsync(nd => nd.DisciplinaId == disciplinaId && nd.Norma.Vacant == true && nd.Norma.StatDefunctieId == statDeFunctieId)
                ?? throw new InvalidOperationException("Nu a fost gasita!");
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

        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId, int statDeFunctieId)
        {
            return await _context.Normas
                .Where(n => n.CadruDidactics.Any(cdgd => cdgd.CadruDidacticGradDidacticId == cadruDidacticId) && n.StatDefunctieId == statDeFunctieId)
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
        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int statDeFunctieId, int cadruDidacticId)
        {
            int anStat = await _context.StatDeFuncties
                .Where(s => s.StatDeFunctieId == statDeFunctieId)
                .Select(s => s.An)
                .FirstOrDefaultAsync();
            System.Console.WriteLine(statDeFunctieId + " " + cadruDidacticId);
            System.Console.WriteLine(anStat);

            var toateNormele = await _context.NormaDisciplinas
                .Include(n => n.Disciplina)
                    .ThenInclude(d => d.AnPromotie)
                .Include(n => n.Norma)
                    .ThenInclude(n => n.CadruDidactics)
                .Where(n => n.Norma.StatDefunctieId == statDeFunctieId)
                .ToListAsync();

            var asociateIds = toateNormele
                .Where(n =>
                    n.Norma.CadruDidactics.Any(cd => cd.CadruDidacticGradDidacticId == cadruDidacticId) ||
                    n.Disciplina.AnPromotie.NumărGrupe == GetNumarGrupeOcupatePentruDisciplina(n.DisciplinaId, statDeFunctieId))
                .Select(n => n.DisciplinaId)
                .ToList();

            System.Console.WriteLine("E null?" + asociateIds.IsNullOrEmpty());

            foreach (var id in asociateIds)
            {
                System.Console.WriteLine("bos: " + id);
            }

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
                        d.AnPromotie.AnDeStudiu + " " + d.AnPromotie.Promotie.AnInceput
            })
            .OrderBy(d => d.Denumire)
            .ToListAsync();

            return disponibile;

        }

        public async Task<Norma> GetNormaByCadruDidacticIdAsync(int cadruDidacticId, int statDeFunctieId)
        {
            return await _context.Normas
                .Include(n => n.CadruDidactics)
                .FirstOrDefaultAsync(n => n.CadruDidactics.Any(cd => cd.CadruDidacticGradDidacticId == cadruDidacticId) && n.StatDefunctieId == statDeFunctieId)
                ?? throw new InvalidOperationException("Norma nu a fost gasita!");

        }

        public async Task<NormaDisciplina> GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(int cadruDidacticId, int disciplinaId)
        {
            return await _context.NormaDisciplinas
                .Include(nd => nd.Norma)
                .FirstOrDefaultAsync(nd => nd.DisciplinaId == disciplinaId && nd.Norma.CadruDidactics.Any(cd => cd.CadruDidacticGradDidacticId == cadruDidacticId))
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

        public async Task<IActionResult> CreazaNormaAsync(int cadruDidacticId, int statDeFunctieId)
        {
            var norma = new Norma
            {
                StatDefunctieId = statDeFunctieId,
                GradDidacticId = 1,
                Vacant = false
            };

            var cadru = await _context.CadruDidacticGradDidactics
                .Where(cdgd => cdgd.PanaLa == null)
                .FirstOrDefaultAsync(cdgd => cdgd.CadruDidacticId == cadruDidacticId);

            if (cadru == null)
            {
                return new BadRequestObjectResult("Nu s-a găsit!.");
            }
            _context.Attach(cadru);
            norma.CadruDidactics.Add(cadru);
            await _context.Normas.AddAsync(norma);
            
            return await _context.SaveChangesAsync() > 0
                ? new OkResult()
                : new BadRequestResult();
        }

        public async Task<List<NormaVacantaCuDisciplineViewModel>> GetNormeVacanteCuDisciplineAsync(int statDeFunctieId)
        {
            return await _context.Normas
                .Where(n => n.StatDefunctieId == statDeFunctieId && n.Vacant == true)
                .Select(n => new NormaVacantaCuDisciplineViewModel
                {
                    NormaId = n.NormaId,
                    DisciplineActivitati = n.NormaDisciplinas
                    .Where(nd => nd.ActivitatePractica == false && nd.ActivitateTeoretica == false)
                    .Select(nd => new DisciplinaActivitateViewModel
                    {
                        DenumireDisciplina = nd.Disciplina.DenumireDisciplina,
                        ActivitateTeoretica = nd.Disciplina.ActivitateTeoretica,
                        ActivitatePractica = nd.Disciplina.ActivitatePractica,
                        ProgramDeStudiu = nd.Disciplina.AnPromotie.Promotie.ProgramDeStudii.Nume,
                        AnStudiu = nd.Disciplina.AnPromotie.AnDeStudiu,
                        NumarGrupe = nd.NumarGrupe,
                        Semestru = nd.Disciplina.SemestruDeDesfasurare,
                        NumarSaptamaniActivitate = nd.Disciplina.NumarSaptamani,
                        KCoeficientActTeoretica = nd.Disciplina.AnPromotie.Promotie.ProgramDeStudii.CoeficientKActivitateTeoretica,
                        KCoeficientActPractica = nd.Disciplina.AnPromotie.Promotie.ProgramDeStudii.CoeficientKActivitatePractica
                    }).ToList()
                })
                .ToListAsync();
        }
        [HttpGet]
        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateLaNormaVacantaAsync(int normaId)
        {
            var disciplineAsociate = await _context.NormaDisciplinas
                .Include(nd => nd.Disciplina)
                .Where(nd => nd.NormaId == normaId && nd.ActivitatePractica == false && nd.ActivitateTeoretica == false)
                .Select(nd => new DisciplinaNormaViewModel
                {
                    DisciplinaId = nd.DisciplinaId,
                    Denumire = nd.Disciplina.DenumireDisciplina,
                    NormaDisciplinaId = nd.NormaDisciplinaId
                })
                .ToListAsync();

            return disciplineAsociate;
        }

        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileLaNormaVacantaAsync(int normaId)
        {

            int anStat = await _context.Normas
                .Where(n => n.NormaId == normaId)
                .Select(n => n.StatDefunctie.An)
                .FirstOrDefaultAsync();

            System.Console.WriteLine("heeeei" + anStat);

            var statDeFunctieId = await _context.Normas
                .Where(n => n.NormaId == normaId)
                .Select(n => n.StatDefunctieId)
                .FirstOrDefaultAsync();

            var toateNormele = await _context.NormaDisciplinas
                .Include(n => n.Disciplina)
                    .ThenInclude(d => d.AnPromotie)
                .Include(n => n.Norma)
                    .ThenInclude(n => n.CadruDidactics)
                .Where(n => n.Norma.StatDefunctieId == statDeFunctieId)
                .ToListAsync();

            var asociateIds = toateNormele
                .Where(nd => (nd.Norma.Vacant == true
                && nd.ActivitatePractica == false
                && nd.ActivitateTeoretica == false)
                || nd.Disciplina.AnPromotie.NumărGrupe == GetNumarGrupeOcupatePentruDisciplina(nd.DisciplinaId, statDeFunctieId))
                .Select(nd => nd.DisciplinaId)
                .ToList();
            

            var disciplineDisponibile = await _context.Disciplinas
                .Where(d => !asociateIds.Contains(d.DisciplinaId)
                && d.AnPromotie.Promotie.AnInceput + (d.AnPromotie.AnDeStudiu - 1) == anStat)
                .OrderByDescending(d => d.AnPromotie.AnDeStudiu)
                .ThenBy(d => d.AnPromotie.Promotie.ProgramDeStudii.Acronim)
                .ThenBy(d => d.DenumireDisciplina)
                .Select(d => new DisciplinaNormaViewModel
                {
                    DisciplinaId = d.DisciplinaId,
                    Denumire = d.DenumireDisciplina + " - " +
                            d.AnPromotie.Promotie.ProgramDeStudii.Acronim + " " +
                            d.AnPromotie.AnDeStudiu + " " + d.AnPromotie.Promotie.AnInceput
                })
                .ToListAsync();

            return disciplineDisponibile;
        }

        public async Task<int> GetNumarGrupeOcupatePentruDisciplinaAsync(int disciplinaId, int statDeFunctieId)
        {
            int numarGrupe = await _context.NormaDisciplinas
            .Where(nd =>
                nd.DisciplinaId == disciplinaId &&
                nd.Norma.StatDefunctieId == statDeFunctieId
                 &&(nd.ActivitatePractica == true
                || (nd.ActivitateTeoretica == false && nd.ActivitatePractica == false))
                )
            .SumAsync(nd => (int?)nd.NumarGrupe ?? 0);

            return numarGrupe;
        }

        public int GetNumarGrupeOcupatePentruDisciplina(int disciplinaId, int statDeFunctieId)
        {
            int numarGrupe = _context.NormaDisciplinas
            .Where(nd =>
                nd.DisciplinaId == disciplinaId &&
                nd.Norma.StatDefunctieId == statDeFunctieId
                &&(nd.ActivitatePractica == true
                || (nd.ActivitateTeoretica == false && nd.ActivitatePractica == false))
                )
            .Sum(nd => (int?)nd.NumarGrupe ?? 0);
            System.Console.WriteLine("Din GenumarGrupe:" + disciplinaId + " " + numarGrupe);
            return numarGrupe;
        }
        public async Task<int> GetNumarGrupeTotal(int disciplinaId, int statDeFunctieId)
        {
            return await _context.Disciplinas
                .Where(d => d.DisciplinaId == disciplinaId)
                .Select(d => d.AnPromotie.NumărGrupe)
                .FirstOrDefaultAsync();
        }

        
    }

}