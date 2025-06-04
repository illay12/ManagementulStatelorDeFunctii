using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ManagementulStatelorDeFunctii.DataLayer.Repos
{
    public class PlataCuOraRepository : IPlataCuOraRepository
    {
        private readonly StateDeFunctieDbbContext _context;
        public PlataCuOraRepository(StateDeFunctieDbbContext context)
        {
            _context = context;
        }

        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineDisponibileAsync(int cadruDidacticId, int statDeFunctieId, int semestru)
        {
            int anStat = await _context.StatDeFuncties
            .Where(s => s.StatDeFunctieId == statDeFunctieId)
            .Select(s => s.An)
            .FirstOrDefaultAsync();

            var idsAsoc = await _context.DisciplinaCadruDidactics
                .Include(dcd => dcd.NormaDisciplina)
                    .ThenInclude(nd => nd.Norma)
                .Where(dcd => dcd.CadruDidacticId == cadruDidacticId
                    && dcd.NormaDisciplina.Norma.StatDefunctieId == statDeFunctieId)
                .Select(dcd => dcd.NormaDisciplina.DisciplinaId)
                .ToListAsync();

            var disciplineDisponibile = await _context.Disciplinas
                .Include(d => d.NormaDisciplinas)
                    .ThenInclude(nd => nd.Norma)
                .Where(d => !idsAsoc.Contains(d.DisciplinaId)
                        && d.NormaDisciplinas.Any(nd => nd.Norma.Vacant == true)
                        && d.AnPromotie.Promotie.AnInceput + (d.AnPromotie.AnDeStudiu - 1) == anStat
                        && d.SemestruDeDesfasurare == semestru
                        && d.NormaDisciplinas.Any(nd => nd.Norma.StatDefunctieId == statDeFunctieId)
                        )
                .Select(d => new DisciplinaNormaViewModel
                {
                    DisciplinaId = d.DisciplinaId,
                    NormaDisciplinaId = 0,
                    Denumire = d.DenumireDisciplina + " - " +
                            d.AnPromotie.Promotie.ProgramDeStudii.Acronim + " " +
                            d.AnPromotie.AnDeStudiu + " " + d.DisciplinaId + " " + d.AnPromotie.Promotie.AnInceput + " " + d.AnPromotie.AnDeStudiu
                })
                .OrderBy(d => d.Denumire)
                .ToListAsync();

            return disciplineDisponibile;
        }

        public async Task<List<CadruDidacticCuDisciplinePlataCuOraViewModel>> GetProfesoriCuDisciplineAsync(int statId, int semestru)
        {
            var rezultate = await _context.DisciplinaCadruDidactics
                .Where(dcd => dcd.NormaDisciplina.Norma.StatDefunctieId == statId &&
                            dcd.NormaDisciplina.Disciplina.SemestruDeDesfasurare == semestru
                            && (dcd.NormaDisciplina.ActivitatePractica == true || dcd.NormaDisciplina.ActivitateTeoretica == true))
                .Include(dcd => dcd.CadruDidactic)
                    .ThenInclude(cd => cd.GradDidactic)
                .Include(dcd => dcd.NormaDisciplina)
                    .ThenInclude(nd => nd.Disciplina)
                        .ThenInclude(d => d.AnPromotie)
                            .ThenInclude(ap => ap.Promotie)
                                .ThenInclude(p => p.ProgramDeStudii)
                .Include(dcd => dcd.NormaDisciplina.Norma)
                .Select(dcd => new
                {
                    CadruDidacticGradDidactic = _context.CadruDidacticGradDidactics
                        .Include(cdgd => cdgd.CadruDidactic)
                        .Include(cdgd => cdgd.GradDidactic)
                        .FirstOrDefault(cdgd => cdgd.CadruDidacticGradDidacticId == dcd.CadruDidacticId),

                    Disciplina = dcd.NormaDisciplina.Disciplina,
                    AnPromotie = dcd.NormaDisciplina.Disciplina.AnPromotie,
                    Promotie = dcd.NormaDisciplina.Disciplina.AnPromotie.Promotie,
                    Program = dcd.NormaDisciplina.Disciplina.AnPromotie.Promotie.ProgramDeStudii,
                    Tarif = _context.CadruDidacticStatFunctieTarifPlataOras
                                .Where(t => t.StatFunctieId == statId && t.CadruDidacticId == dcd.CadruDidacticId)
                                .Select(t => t.Tarif)
                                .FirstOrDefault(),
                    NumarGrupe = dcd.NormaDisciplina.NumarGrupe
                })
                .Where(x => x.CadruDidacticGradDidactic != null && x.CadruDidacticGradDidactic.CadruDidactic != null)
                .ToListAsync();

            var grupat = rezultate
                .GroupBy(x => new
                {
                    x.CadruDidacticGradDidactic.CadruDidacticGradDidacticId,
                    Nume = x.CadruDidacticGradDidactic.CadruDidactic.Nume,
                    Grad = x.CadruDidacticGradDidactic.GradDidactic?.NumeGrad ?? "Nedefinit",
                    Tarif = x.Tarif
                })
                .Select(g => new CadruDidacticCuDisciplinePlataCuOraViewModel
                {
                    CadruDidacticId = g.Key.CadruDidacticGradDidacticId,
                    NumeProfesor = g.Key.Nume,
                    Grad = g.Key.Grad,
                    TarifPlataCuOra = g.Key.Tarif,


                    DisciplineActivitati = g.Select(x => new DisciplinaActivitateViewModel
                    {
                        DenumireDisciplina = x.Disciplina.DenumireDisciplina,
                        ActivitateTeoretica = x.Disciplina.ActivitateTeoretica,
                        ActivitatePractica = x.Disciplina.ActivitatePractica,
                        ProgramDeStudiu = x.Program?.Nume ?? "Necunoscut",
                        AnStudiu = x.Disciplina.AnPromotie.AnDeStudiu,
                        NumarGrupe = x.NumarGrupe,
                        Semestru = x.Disciplina.SemestruDeDesfasurare,
                        NumarSaptamaniActivitate = x.Disciplina.NumarSaptamani,
                        KCoeficientActTeoretica = x.Program?.CoeficientKActivitateTeoretica ?? 1,
                        KCoeficientActPractica = x.Program?.CoeficientKActivitatePractica ?? 1,
                        OrePeSemestru = (x.Disciplina.ActivitateTeoretica * (x.Program?.CoeficientKActivitateTeoretica ?? 1) +
                                        x.Disciplina.ActivitatePractica * (x.Program?.CoeficientKActivitatePractica ?? 1) * x.NumarGrupe)
                                        * x.AnPromotie.NumarSaptamaniActivitate,
                        PlataPeSemestru = (x.Disciplina.ActivitateTeoretica * (x.Program?.CoeficientKActivitateTeoretica ?? 1) +
                                        x.Disciplina.ActivitatePractica * (x.Program?.CoeficientKActivitatePractica ?? 1) * x.NumarGrupe)
                                        * x.AnPromotie.NumarSaptamaniActivitate * g.Key.Tarif

                    }).ToList(),

                    TotalOrePeSemestru = g.Select(x =>
                        (x.Disciplina.ActivitateTeoretica * (x.Program?.CoeficientKActivitateTeoretica ?? 1) +
                        x.Disciplina.ActivitatePractica * (x.Program?.CoeficientKActivitatePractica ?? 1) * x.NumarGrupe)
                        * x.AnPromotie.NumarSaptamaniActivitate
                    ).Sum(),

                    TotalPlataPeSemestru = g.Select(x =>
                        (x.Disciplina.ActivitateTeoretica * (x.Program?.CoeficientKActivitateTeoretica ?? 1) +
                        x.Disciplina.ActivitatePractica * (x.Program?.CoeficientKActivitatePractica ?? 1) * x.NumarGrupe)
                        * x.AnPromotie.NumarSaptamaniActivitate * g.Key.Tarif
                    ).Sum(),

                })
                .ToList();

            return grupat;
        }

        public async Task<List<DisciplinaNormaViewModel>> GetDisciplineAsociateAsync(int cadruDidacticId, int statDeFunctieId, int semestru)
        {
            var disciplineAsociate = await _context.DisciplinaCadruDidactics
                .Where(dcd => dcd.CadruDidacticId == cadruDidacticId
                && dcd.NormaDisciplina.Norma.StatDefunctieId == statDeFunctieId
                && dcd.NormaDisciplina.Norma.Vacant == true
                && dcd.NormaDisciplina.Disciplina.SemestruDeDesfasurare == semestru)
                .Include(dcd => dcd.NormaDisciplina)
                    .ThenInclude(nd => nd.Disciplina)
                .Select(dcd => new DisciplinaNormaViewModel
                {
                    DisciplinaId = dcd.NormaDisciplina.Disciplina.DisciplinaId,
                    NormaDisciplinaId = dcd.NormaDisciplina.NormaDisciplinaId,
                    Denumire = dcd.NormaDisciplina.Disciplina.DenumireDisciplina
                })
                .ToListAsync();

            return disciplineAsociate;
        }

        public async Task<DisciplinaCadruDidactic> GetDisciplinaCadruDidacticByNormaDiscIdCadruId(int normaDisciplinaId, int cadruDidacticId)
        {
            return await _context.DisciplinaCadruDidactics.FirstOrDefaultAsync(d => d.CadruDidacticId == cadruDidacticId && d.NormaDisciplinaId == normaDisciplinaId);
        }

        // public Task<IActionResult> AdaugaCadruDidacticLaPlataCuOra(int statDeFunctieId, int semestru)
        // {

        // }

        public async Task<IEnumerable<CadruDidacticGradDidactic>> GetCadreDidacticeDisponibileLaAdaugare(int statDeFunctieId, int semestru)
        {
            var idsAsociate = await _context.DisciplinaCadruDidactics
                .Where(dcd => dcd.NormaDisciplina.Norma.StatDefunctieId == statDeFunctieId &&
                            dcd.NormaDisciplina.Disciplina.SemestruDeDesfasurare == semestru)
                .Select(dcd => dcd.CadruDidactic.CadruDidacticGradDidacticId)
                .ToListAsync();

            var cadreDidacticeDisponibile = await _context.CadruDidacticGradDidactics
                .Include(cdgd => cdgd.CadruDidactic)
                .Where(cdgd => !idsAsociate.Contains(cdgd.CadruDidacticGradDidacticId) && cdgd.PanaLa == null)
                .ToListAsync();

            return cadreDidacticeDisponibile;
        }

        public async Task<int> GetCadruDidacticGradDidacticIDLaPlataCuOraByCadruDidacticIdAsync(int cadruDidacticId)
        {
            var cadruDidactic = await _context.CadruDidacticGradDidactics
                .Where(c => c.CadruDidacticId == cadruDidacticId && c.PanaLa == null)
                .Select(c => c.CadruDidacticGradDidacticId)
                .FirstOrDefaultAsync();

            System.Console.WriteLine("Din GetCadruDidacticGradDidacticIDLaPlataCuOraByCadruDidacticIdAsync: CadruDidacticGradDidacticId " + cadruDidactic);

            return cadruDidactic;
        }
        public async Task<int> GetGrupeDisponibileDinNormaVacantaAsync(int disciplinaId, int statDeFunctieId)
        {
            return await _context.NormaDisciplinas
                .Where(nd => nd.DisciplinaId == disciplinaId
                && nd.Norma.StatDefunctieId == statDeFunctieId
                && nd.Norma.Vacant == true)
                .Select(nd => nd.NumarGrupe)
                .SumAsync();
                
        }


    }
}
