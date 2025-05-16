using ManagementulStatelorDeFunctii.Models;
using Microsoft.EntityFrameworkCore;
public class CadruDidacticRepository : ICadruDidacticRepository
{
    private readonly StateDeFunctieDbbContext _context;

    public CadruDidacticRepository(StateDeFunctieDbbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CadruDidactic>> GetAllAsync() =>
        await _context.CadruDidactics
            .Include(c => c.GradDidactic)
            .Include(c => c.Departament)
            .ToListAsync();

    public async Task<CadruDidactic?> GetByIdAsync(int id) =>
        await _context.CadruDidactics
            .Include(c => c.GradDidactic)
            .Include(c => c.Departament)
            .FirstOrDefaultAsync(c => c.CadruDidacticId == id);

    public async Task AddAsync(CadruDidactic cadru)
    {
        _context.CadruDidactics.Add(cadru);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CadruDidactic cadru)
    {
        _context.CadruDidactics.Update(cadru);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cadru = await _context.CadruDidactics.FindAsync(id);
        if (cadru != null)
        {
            _context.CadruDidactics.Remove(cadru);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CadruDidactic>> GetCadreDidacticeDisponibilePentruAsociereNormaAsync(int statDeFunctieId)
    {
        var cadreDisponibileIds = await _context.CadruDidacticGradDidactics
            .Where(cd => cd.Normas.Any(n => n.StatDefunctieId == statDeFunctieId))
            .Select(cd => cd.CadruDidacticId)
            .ToListAsync();

                
        var cadreDisponibile = await _context.CadruDidactics
            .Where(cd => !cadreDisponibileIds.Contains(cd.CadruDidacticId) &&
                        _context.CadruDidacticGradDidactics
                            .Any(cg => cg.CadruDidacticId == cd.CadruDidacticId))
            .ToListAsync();


        foreach (var cadru in cadreDisponibile)
            System.Console.WriteLine(cadru.CadruDidacticId + " " + cadru.Nume);

        return cadreDisponibile;
    
    }

}

