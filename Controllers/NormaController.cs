using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManagementulStatelorDeFunctii.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ManagementulStatelorDeFunctii.Controllers
{
    public class NormaController : Controller
    {
        private readonly IGenericRepository<Disciplina> _disciplinaRepository;
        private readonly INormaRepository _normaRepository;
        private readonly IGenericRepository<NormaDisciplina> _normaDisciplinaRepository;
        private readonly ICadruDidacticRepository _cadruDidacticRepository;

        public NormaController(
            INormaRepository normaRepository,
            IGenericRepository<Disciplina> disciplinaRepository,
            IGenericRepository<NormaDisciplina> normaDisciplinaRepository,
            ICadruDidacticRepository cadruDidacticRepository)
        {
            _disciplinaRepository = disciplinaRepository;
            _normaRepository = normaRepository;
            _normaDisciplinaRepository = normaDisciplinaRepository;
            _cadruDidacticRepository = cadruDidacticRepository;
        }

        [HttpGet]
        public async Task<IActionResult> AsociazaDisciplinaNorma(int cadruDidacticId, int disciplinaId)
        {
            var Disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);
            System.Console.WriteLine(cadruDidacticId + " " + disciplinaId);
            if (Disciplina == null)
                return NotFound("Disciplina nu a fost găsită.");

            ViewBag.DenumireDisciplina = Disciplina.DenumireDisciplina;

            var model = new AsociereDisciplinaNormaViewModel
            {
                CadruDidacticId = cadruDidacticId,
                DisciplinaId = disciplinaId
            };
            Console.WriteLine($"GET: cadruDidacticId={cadruDidacticId}, disciplinaId={disciplinaId}");

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AsociazaDisciplinaNorma(AsociereDisciplinaNormaViewModel model)
        {
            System.Console.WriteLine("Model: " + model.DisciplinaId);
            if (!ModelState.IsValid)
            {
                var denumireDisciplina = await _disciplinaRepository.GetByIdAsync(model.DisciplinaId);
                ViewBag.DenumireDisciplina = denumireDisciplina?.DenumireDisciplina ?? "N/A";
                return View(model);
            }

            var norma = await _normaRepository.GetNormaByCadruDidacticIdAsync(model.CadruDidacticId, (int)TempData["StatDeFunctieId"]);

            var nd_DeAdaugat = new NormaDisciplina
            {
                NormaId =  norma.NormaId,
                DisciplinaId = model.DisciplinaId,
                NumarSerii = model.NumarSerii,
                NumarGrupe = model.NumarGrupe,
                ActivitateTeoretica = model.AreActivitateTeoretica,
                ActivitatePractica = model.AreActivitatePractica,
                
            };

            await _normaDisciplinaRepository.AddAsync(nd_DeAdaugat);

            return RedirectToAction("Edit", new { cadruDidacticId = model.CadruDidacticId, statDeFunctieId = TempData["StatDeFunctieId"] });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int cadruDidacticId, int statDeFunctieId)
        {
            ViewBag.StatDeFunctieId = statDeFunctieId;
            ViewBag.CadruDidacticId = cadruDidacticId;
            TempData["StatDeFunctieId"] = statDeFunctieId;
            System.Console.WriteLine("StatDeFunctieId: " + statDeFunctieId);

            var disciplineDisponibile = (await _normaRepository.GetDisciplineDisponibileAsync(statDeFunctieId,cadruDidacticId))
            .Select(d => new SelectListItem
            {
                Value = d.DisciplinaId.ToString(),
                Text = d.Denumire
            }).ToList();

            var viewModel = new EditareNormaViewModel
            {
                CadruDidacticId = cadruDidacticId,
                StatDeFunctieId = statDeFunctieId,
                DisciplineAsociate = await _normaRepository.GetDisciplineAsociateAsync(cadruDidacticId,statDeFunctieId),
                DisciplineDisponibile = disciplineDisponibile
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EliminaDisciplina(int cadruDidacticId, int disciplinaId,int statDeFunctieId)
        {
            var normaDisciplina = await _normaRepository.GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(cadruDidacticId, disciplinaId);
            
            await _normaRepository.EliminaAsociereDisciplinaDeLaCadruAsync(normaDisciplina.NormaDisciplinaId);
            
            return RedirectToAction("Edit", new { cadruDidacticId, statDeFunctieId });
        }

        [HttpGet]
        public async Task<IActionResult> CreazaNorma(int statDeFunctieId)
        {
            var cadre = await _cadruDidacticRepository.GetCadreDidacticeDisponibilePentruAsociereNormaAsync(statDeFunctieId);

            var model = new CreazaNormaViewModel
            {
                StatDeFunctieId = statDeFunctieId,
                CadreDidacticeDisponibile = cadre
                    .Select(cd => new SelectListItem
                    {
                        Value = cd.CadruDidacticId.ToString(),
                        Text = cd.Nume
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreazaNorma(CreazaNormaViewModel model)
        {
            System.Console.WriteLine("Model: " + model.CadruDidacticId + " " + model.StatDeFunctieId);
            await _normaRepository.CreazaNormaAsync(model.CadruDidacticId,model.StatDeFunctieId);
            return RedirectToAction("Index", "StatDeFunctie", new { id = model.StatDeFunctieId });
        }
        // [HttpPost]
        // public async Task<IActionResult> AdaugaDisciplina(int cadruDidacticId, int normaDisciplinaId, int statDeFunctieId)
        // {
        //     await _normaRepository.AsociazaDisciplinaLaCadruAsync(cadruDidacticId, normaDisciplinaId);
        //     return RedirectToAction("Edit", new { cadruDidacticId, statDeFunctieId });
        // }

    }
}
