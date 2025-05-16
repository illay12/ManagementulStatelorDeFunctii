using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;

public interface IStatDeFunctieRepository
{
    Task<List<ProfesorDisciplinePeStatViewModel>> GetProfesoriCuDisciplineAsync(int statId);
    Task<List<StatDeFunctie>> GetAllStateAsync();

}
