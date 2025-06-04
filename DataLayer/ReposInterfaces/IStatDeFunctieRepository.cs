using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using Microsoft.AspNetCore.Mvc;

public interface IStatDeFunctieRepository : IGenericRepository<StatDeFunctie>
{
    Task<List<ProfesorDisciplinePeStatViewModel>> GetProfesoriCuDisciplineAsync(int statId);
    Task<List<StatDeFunctie>> GetAllStateAsync();
    Task<StatDeFunctie?> GetById(int id);
    Task<bool> SaveChangesAsync();
    Task<IActionResult> StergeStatDeFunctie(int statDeFunctieId);



}
