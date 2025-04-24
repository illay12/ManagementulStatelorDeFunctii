using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class StatFunctieCadruViewModel
{
    [Required]
    public int CadruDidacticId { get; set; }
    public int GradDidacticId { get; set;}
    public int StatDeFunctieId { get; set; }
    public int DataDeLa { get; set; }
    public int? DataPanaLa { get; set; }
    public decimal Tarif {get; set;}
    public IEnumerable<SelectListItem>? CadreDisponibile { get; set; }
    public IEnumerable<SelectListItem>? StateDisponibile { get; set; }
    public IEnumerable<SelectListItem>? GradeDisponibile { get; set; }

}
