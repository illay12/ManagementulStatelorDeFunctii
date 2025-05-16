using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EditareNormaViewModel
{
    public int CadruDidacticId { get; set; }
    public int StatDeFunctieId { get; set; }
    public string NumeProfesor { get; set; }
    public List<DisciplinaNormaViewModel> DisciplineAsociate { get; set; } = new();
    public List<SelectListItem> DisciplineDisponibile { get; set; } = new();
}
