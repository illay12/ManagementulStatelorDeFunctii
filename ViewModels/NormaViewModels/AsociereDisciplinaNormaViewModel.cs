using System.ComponentModel.DataAnnotations;

public class AsociereDisciplinaNormaViewModel
{
    public int CadruDidacticId { get; set; }
    public int StatDeFunctieId { get; set; }
    public int DisciplinaId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Numărul de serii trebuie să fie cel puțin 1.")]
    public int NumarSerii { get; set; }
    public int NumarGrupe { get; set; }
    public bool AreActivitateTeoretica { get; set; }
    public bool AreActivitatePractica { get; set; }

}
