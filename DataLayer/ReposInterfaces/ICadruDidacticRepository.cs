using ManagementulStatelorDeFunctii.Models;

public interface ICadruDidacticRepository
{
    Task<IEnumerable<CadruDidactic>> GetAllAsync();
    Task<CadruDidactic?> GetByIdAsync(int id);
    Task AddAsync(CadruDidactic cadru);
    Task UpdateAsync(CadruDidactic cadru);
    Task DeleteAsync(int id);
}
