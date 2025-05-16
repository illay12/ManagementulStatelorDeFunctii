using System.ComponentModel.DataAnnotations;

public class AsociereDisciplinaNormaViewModel
{
    public int CadruDidacticId { get; set; }
    public int DisciplinaId { get; set; }
    public int NumarSerii { get; set; }
    public int NumarGrupe { get; set; }
    public bool AreActivitateTeoretica { get; set; }
    public bool AreActivitatePractica { get; set; }

}
