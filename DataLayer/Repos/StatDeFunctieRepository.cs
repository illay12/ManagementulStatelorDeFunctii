using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ManagementulStatelorDeFunctii.DataLayer.Repos
{
public class StatDeFunctieRepository : IStatDeFunctieRepository
{
    private readonly StateDeFunctieDbbContext _context;

    public StatDeFunctieRepository(StateDeFunctieDbbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfesorDisciplinePeStatViewModel>> GetProfesoriCuDisciplineAsync(int statId)
    {
        var norme = await _context.Normas
            .Where(n => n.StatDefunctieId == statId)
            .Include(n => n.NormaDisciplinas)
                .ThenInclude(nd => nd.Disciplina)
                    .ThenInclude(d => d.AnPromotie)
                        .ThenInclude(ap => ap.Promotie)
                            .ThenInclude(p => p.ProgramDeStudii)
            .Include(n => n.CadruDidactics)
                .ThenInclude(cdgd => cdgd.CadruDidactic)
            .Include(n => n.CadruDidactics)
                .ThenInclude(cdgd => cdgd.GradDidactic)
            .ToListAsync();

        var rezultat = norme
            .SelectMany(n => n.CadruDidactics.Select(cdgd => new
            {
                CadruDidacticId = cdgd.CadruDidacticId,
                Profesor = cdgd.CadruDidactic?.Nume,
                Grad = cdgd.GradDidactic?.NumeGrad,
                Discipline = n.NormaDisciplinas.Select(nd => new DisciplinaActivitateViewModel
                {
                    DenumireDisciplina = nd.Disciplina?.DenumireDisciplina,
                    ActivitateTeoretica = nd.ActivitateTeoretica ? nd.Disciplina.ActivitateTeoretica : 0,
                    ActivitatePractica = nd.ActivitatePractica ? nd.Disciplina.ActivitatePractica : 0,
                    ProgramDeStudiu = nd.Disciplina?.AnPromotie?.Promotie?.ProgramDeStudii?.Nume,
                    AnStudiu = nd.Disciplina?.AnPromotie?.AnDeStudiu ?? 0,
                    NumarGrupe = nd.NumarGrupe,
                    Semestru = nd.Disciplina?.SemestruDeDesfasurare ?? 0
                }).ToList()
            }))
            .Where(x => x.Profesor != null && x.Grad != null)
            .GroupBy(x => new { x.CadruDidacticId, x.Profesor, x.Grad,statId })
            .Select(g => new ProfesorDisciplinePeStatViewModel
            {
                CadruDidacticId = g.Key.CadruDidacticId,
                NumeProfesor = g.Key.Profesor,
                Grad = g.Key.Grad,
                DisciplineActivitati = g.SelectMany(x => x.Discipline).Where(d => d.DenumireDisciplina != null).ToList()
            })
            .ToList();

        return rezultat;
    }

public async Task<List<StatDeFunctie>> GetAllStateAsync()
{
    return await _context.StatDeFuncties.ToListAsync();
}



}

}