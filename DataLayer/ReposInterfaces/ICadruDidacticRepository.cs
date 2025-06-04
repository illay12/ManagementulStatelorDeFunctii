using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels.CadruDidacticViewModels;

public interface ICadruDidacticRepository
{
    Task<IEnumerable<CadruDidactic>> GetAllAsync();
    Task<CadruDidactic?> GetByIdAsync(int id);
    Task AddAsync(CadruDidactic cadru);
    Task UpdateAsync(CadruDidactic cadru);
    Task DeleteAsync(int id);
    Task<IEnumerable<CadruDidactic>> GetCadreDidacticeDisponibilePentruAsociereNormaAsync(int statDeFunctieId);
    Task<CadruDidactic> GetCadruDidaticByCadruDidacticGradDidacticIdAsync(int cadruDidacticGradDidacticId);
    Task<List<CadruDidacticCompletViewModel>> GetCadreDidacticeCompletAsync();

}
