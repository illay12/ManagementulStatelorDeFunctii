using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.Repos;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels.PlataCuOraViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.Controllers
{
    public class PlataCuOraController : Controller
    {
        private readonly IPlataCuOraRepository _plataCuOraRepository;
        private readonly INormaRepository _normaRepository;
        private readonly ICadruDidacticRepository _cadruDidacticRepository;
        private readonly IGenericRepository<Disciplina> _disciplinaRepository;
        private readonly IGenericRepository<DisciplinaCadruDidactic> _disciplinaCadruDidacticRepository;
        private readonly IGenericRepository<NormaDisciplina> _normaDisciplinaRepository;
        private readonly IGenericRepository<CadruDidacticGradDidactic> _cadruDidacticGradDidacticRepository;
        public PlataCuOraController(IPlataCuOraRepository plataCuOraRepository,
            INormaRepository normaRepository,
            ICadruDidacticRepository cadruDidacticRepository,
            IGenericRepository<CadruDidacticGradDidactic> cadruDidacticGradDidacticRepository,
            IGenericRepository<Disciplina> disciplinaRepository,
            IGenericRepository<DisciplinaCadruDidactic> disciplinaCadruDidacticRepository,
            IGenericRepository<NormaDisciplina> normaDisciplinaRepository)
        {
            _cadruDidacticRepository = cadruDidacticRepository;
            _plataCuOraRepository = plataCuOraRepository;
            _normaRepository = normaRepository;
            _disciplinaRepository = disciplinaRepository;
            _disciplinaCadruDidacticRepository = disciplinaCadruDidacticRepository;
            _normaDisciplinaRepository = normaDisciplinaRepository;
            _cadruDidacticGradDidacticRepository = cadruDidacticGradDidacticRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int statDeFunctieId, int semestru)
        {

            ViewBag.StatDeFunctieId = statDeFunctieId;
            ViewBag.Semestru = semestru;
            var profesori = await _plataCuOraRepository.GetProfesoriCuDisciplineAsync(statDeFunctieId, semestru);
            return View(profesori);
        }

        [HttpGet]
        public async Task<IActionResult> EditareCadruPlataCuOra(int cadruDidacticId, int statDeFunctieId, int semestru)
        {
            System.Console.WriteLine("Din EditareCadruPlataCuOra2:cadruDidacticGradId " + cadruDidacticId);
            ViewBag.StatDeFunctieId = statDeFunctieId;
            TempData["StatDeFunctieId"] = statDeFunctieId;
            TempData["Semestru"] = semestru;

            var disciplineDisponibile = (await _plataCuOraRepository.GetDisciplineDisponibileAsync(cadruDidacticId, statDeFunctieId, semestru))
            .Select(d => new SelectListItem
            {
                Value = d.DisciplinaId.ToString(),
                Text = d.Denumire
            }).ToList();

            var prof = await _cadruDidacticRepository.GetCadruDidaticByCadruDidacticGradDidacticIdAsync(cadruDidacticId);
            if (prof == null)
            {
                return NotFound("Cadru didactic nu a fost găsit.");
            }

            var viewModel = new EditareCadruPlataCuOraViewModel
            {
                CadruDidacticId = cadruDidacticId,
                NumeProfesor = prof.Nume,
                Semestru = semestru,
                StatDeFunctieId = statDeFunctieId,
                DisciplineAsociate = await _plataCuOraRepository.GetDisciplineAsociateAsync(cadruDidacticId, statDeFunctieId, semestru),
                DisciplineDisponibile = disciplineDisponibile
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AsociazaDisciplinaLaPlataCuOra(int cadruDidacticId, int disciplinaId)
        {
            var Disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);

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
        public async Task<IActionResult> AsociazaDisciplinaLaPlataCuOra(AsociereDisciplinaNormaViewModel model)
        {
            var normaDisciplinaDinNormaVacanta = await _normaRepository.GetNormaDisciplinaVacantaByDisciplinaIdAsync(model.DisciplinaId, (int)TempData["StatDeFunctieId"]);
            System.Console.WriteLine("Din GetCadruDidacticGradDidacticIDLaPlataCuOraByCadruDidacticIdAsync: CadruDidacticGradDidacticId " + model.CadruDidacticId);   

            var normaDisciplina = new NormaDisciplina
            {
                DisciplinaId = model.DisciplinaId,
                NormaId = normaDisciplinaDinNormaVacanta.NormaId,
                NumarSerii = model.NumarSerii,
                NumarGrupe = model.NumarGrupe,
                ActivitateTeoretica = model.AreActivitateTeoretica,
                ActivitatePractica = model.AreActivitatePractica
            };
            await _normaDisciplinaRepository.AddAsync(normaDisciplina);

            var disciplinaCadruDidactic = new DisciplinaCadruDidactic
            {
                CadruDidacticId = model.CadruDidacticId,
                NormaDisciplinaId = normaDisciplina.NormaDisciplinaId,
            };

            await _disciplinaCadruDidacticRepository.AddAsync(disciplinaCadruDidactic);
            return RedirectToAction("EditareCadruPlataCuOra", new { model.CadruDidacticId, statDeFunctieId = (int)TempData["StatDeFunctieId"], semestru = (int)TempData["Semestru"] });
        }

        [HttpPost]
        public async Task<IActionResult> StergeDisciplinaPlataCuOra(int cadruDidacticId, int statDeFunctieId, int disciplinaId)
        {
            var normaDisciplina = await _normaRepository.GetNormaDisciplinaAsociataCadruAsync(cadruDidacticId,disciplinaId,statDeFunctieId);
            System.Console.WriteLine(normaDisciplina.NormaDisciplinaId);
            var disciplinaCadruDidactic = await _plataCuOraRepository.GetDisciplinaCadruDidacticByNormaDiscIdCadruId(normaDisciplina.NormaDisciplinaId,cadruDidacticId);

            await _disciplinaCadruDidacticRepository.DeleteAsync(disciplinaCadruDidactic.NormaDisciplinaCadruDidacticId);

            return RedirectToAction("EditareCadruPlataCuOra", new { cadruDidacticId, statDeFunctieId = TempData["StatDeFunctieId"], semestru = (int)TempData["Semestru"] });
        }

        [HttpGet]
        public async Task<IActionResult> AdaugaCadruLaPlataCuOra(int statDeFunctieId, int semestru)
        {
            var cadreDidacticeDisponibile = await _plataCuOraRepository.GetCadreDidacticeDisponibileLaAdaugare(statDeFunctieId, semestru);
            var viewModel = new AdaugaCadruLaPlataCuOraViewModel
            {
                Semestru = semestru,
                StatDeFunctieId = statDeFunctieId,
                CadreDidacticeDisponibile = cadreDidacticeDisponibile.Select(c => new SelectListItem
                {
                    Value = c.CadruDidacticGradDidacticId.ToString(),
                    Text = c.CadruDidactic.Nume
                }).ToList()
            };

            return View(viewModel);
        }

    }

}