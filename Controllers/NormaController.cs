using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.ViewModels.NormaViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManagementulStatelorDeFunctii.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ManagementulStatelorDeFunctii.ViewModels.NormaVacantaViewModels;

namespace ManagementulStatelorDeFunctii.Controllers
{
    public class NormaController : Controller
    {
        private readonly IGenericRepository<Disciplina> _disciplinaRepository;
        private readonly INormaRepository _normaRepository;
        private readonly IGenericRepository<NormaDisciplina> _normaDisciplinaRepository;
        private readonly ICadruDidacticRepository _cadruDidacticRepository;
        private readonly IGenericRepository<Norma> _genereicNormaRepository;
        private readonly IGenericRepository<AnPromotie> _anPromotieRepository;
        private readonly IGenericRepository<CadruDidacticGradDidactic> _cadruDidacticGradDidacticRepository;


        public NormaController(
            INormaRepository normaRepository,
            IGenericRepository<Disciplina> disciplinaRepository,
            IGenericRepository<NormaDisciplina> normaDisciplinaRepository,
            ICadruDidacticRepository cadruDidacticRepository,
            IGenericRepository<Norma> genereicNormaRepository,
            IGenericRepository<AnPromotie> anPromotieRepository,
            IGenericRepository<CadruDidacticGradDidactic> cadruDidacticGradDidacticRepository

            )
        {
            _disciplinaRepository = disciplinaRepository;
            _normaRepository = normaRepository;
            _normaDisciplinaRepository = normaDisciplinaRepository;
            _cadruDidacticRepository = cadruDidacticRepository;
            _genereicNormaRepository = genereicNormaRepository;
            _anPromotieRepository = anPromotieRepository;
            _cadruDidacticGradDidacticRepository = cadruDidacticGradDidacticRepository;
        }

        [HttpGet]
        public async Task<IActionResult> AsociazaDisciplinaNorma(int cadruDidacticId, int disciplinaId, int statDeFunctieId)
        {
            var Disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);
            if (Disciplina == null)
                return NotFound("Disciplina nu a fost găsită.");

            var anPromotie = await _anPromotieRepository.GetByIdAsync(Disciplina.AnPromotieId);

            int grupeTotale = anPromotie?.NumărGrupe ?? 0;
            int seriiTotale = anPromotie?.NumarSerii ?? 0;

            var grupeFolosite = await _normaRepository.GetNumarGrupeOcupatePentruDisciplinaAsync(disciplinaId, statDeFunctieId);

            int grupeRamase = Math.Max(grupeTotale - grupeFolosite, 0);

            ViewBag.DenumireDisciplina = Disciplina.DenumireDisciplina;
            ViewBag.GrupeDisponibile = Enumerable.Range(1, grupeRamase)
            .Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = $"{i} grupe"
            }).ToList();

            var model = new AsociereDisciplinaNormaViewModel
            {
                CadruDidacticId = cadruDidacticId,
                DisciplinaId = disciplinaId,
                StatDeFunctieId = statDeFunctieId,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AsociazaDisciplinaNorma(AsociereDisciplinaNormaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var denumireDisciplina = await _disciplinaRepository.GetByIdAsync(model.DisciplinaId);
                ViewBag.DenumireDisciplina = denumireDisciplina?.DenumireDisciplina ?? "N/A";
                return View(model);
            }
            
            var norma = await _normaRepository.GetNormaByCadruDidacticIdAsync(model.CadruDidacticId, model.StatDeFunctieId);
            if (model.AreActivitatePractica == false)
            {
                model.NumarGrupe = await _normaRepository.GetNumarGrupeTotal(model.DisciplinaId, model.StatDeFunctieId);
            }

            var nd_DeAdaugat = new NormaDisciplina
            {
                NormaId = norma.NormaId,
                DisciplinaId = model.DisciplinaId,
                NumarSerii = model.NumarSerii,
                NumarGrupe = model.NumarGrupe,
                ActivitateTeoretica = model.AreActivitateTeoretica,
                ActivitatePractica = model.AreActivitatePractica,

            };

            await _normaDisciplinaRepository.AddAsync(nd_DeAdaugat);

            return RedirectToAction("Edit", new { cadruDidacticId = model.CadruDidacticId, model.StatDeFunctieId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int cadruDidacticId, int statDeFunctieId)
        {
            ViewBag.StatDeFunctieId = statDeFunctieId;
            ViewBag.CadruDidacticId = cadruDidacticId;
            TempData["StatDeFunctieId"] = statDeFunctieId;
        

            var disciplineDisponibile = (await _normaRepository.
                GetDisciplineDisponibileAsync(statDeFunctieId, cadruDidacticId))
            .Select(d => new SelectListItem
            {
                Value = d.DisciplinaId.ToString(),
                Text = d.Denumire
            }).ToList();

            var numeProf = (await _cadruDidacticRepository
                        .GetByIdAsync((await _cadruDidacticGradDidacticRepository
                        .GetByIdAsync(cadruDidacticId)).CadruDidacticId))?.Nume;
            
            var viewModel = new EditareNormaViewModel
            {
                NumeProfesor = numeProf,
                CadruDidacticId = cadruDidacticId,
                StatDeFunctieId = statDeFunctieId,
                DisciplineAsociate = await _normaRepository
                    .GetDisciplineAsociateAsync(cadruDidacticId, statDeFunctieId),
                DisciplineDisponibile = disciplineDisponibile
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EliminaDisciplina(int cadruDidacticId, int disciplinaId, int statDeFunctieId)
        {
            var normaDisciplina = await _normaRepository.GetNormaDisciplinaByCadruDidacticDisciplinaIdAsync(cadruDidacticId, disciplinaId);

            await _normaRepository.EliminaAsociereDisciplinaDeLaCadruAsync(normaDisciplina.NormaDisciplinaId);

            return RedirectToAction("Edit", new { cadruDidacticId, statDeFunctieId });
        }

        [HttpGet]
        public async Task<IActionResult> CreazaNorma(int statDeFunctieId)
        {
            var cadre = await _cadruDidacticRepository
            .GetCadreDidacticeDisponibilePentruAsociereNormaAsync(statDeFunctieId);

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
            await _normaRepository.CreazaNormaAsync(model.CadruDidacticId, model.StatDeFunctieId);
            return RedirectToAction("Index", "StatDeFunctie", new { id = model.StatDeFunctieId });
        }

        [HttpGet]
        public async Task<IActionResult> AfiseazaNormeVacante(int statDeFunctieId)
        {
            var normeVacante = await _normaRepository.GetNormeVacanteCuDisciplineAsync(statDeFunctieId);

            ViewBag.StatDeFunctieId = statDeFunctieId;

            return View("~/Views/NormaVacanta/AfiseazaNormeVacante.cshtml", normeVacante);
        }

        [HttpPost]
        public async Task<IActionResult> CreeazaNormaVacanta(int statDeFunctieId)
        {
            var normaDeAdaugat = new Norma
            {
                StatDefunctieId = statDeFunctieId,
                Vacant = true,
                GradDidacticId = 1
            };
            await _genereicNormaRepository.AddAsync(normaDeAdaugat);
            return RedirectToAction("AfiseazaNormeVacante", new { statDeFunctieId = statDeFunctieId });


        }

        [HttpGet]
        public async Task<IActionResult> EditareNormaVacanta(int normaId, int statDeFunctieId)
        {
            ViewBag.NormaId = normaId;
            ViewBag.StatDeFunctieId = statDeFunctieId;
            TempData["StatDeFunctieId"] = statDeFunctieId;

            var disciplineAsociate = await _normaRepository.GetDisciplineAsociateLaNormaVacantaAsync(normaId);

            var disciplineDisponibile = (await _normaRepository.GetDisciplineDisponibileLaNormaVacantaAsync(normaId))
            .Select(d => new SelectListItem
            {
                Value = d.DisciplinaId.ToString(),
                Text = d.Denumire
            }).ToList();

            var viewModel = new EditareNormaVacantaViewModel
            {
                NormaId = normaId,
                StatDeFunctieId = statDeFunctieId,
                DisciplineAsociate = disciplineAsociate,
                DisciplineDisponibile = disciplineDisponibile
            };

            return View("~/Views/NormaVacanta/EditareNormaVacanta.cshtml", viewModel);
        }
        
        
        [HttpGet]
        public async Task<IActionResult> AsociazaDisciplinaNormaVacanta(int disciplinaId,int normaId)
        {
            var Disciplina = await _disciplinaRepository.GetByIdAsync(disciplinaId);
            if (Disciplina == null)
                return NotFound("Disciplina nu a fost găsită.");
            ViewBag.DenumireDisciplina = Disciplina.DenumireDisciplina;

            var anPromotie = await _anPromotieRepository.GetByIdAsync(Disciplina.AnPromotieId);

            int grupeTotale = anPromotie?.NumărGrupe ?? 0;
            int seriiTotale = anPromotie?.NumarSerii ?? 0;

            var norma = await _genereicNormaRepository.GetByIdAsync(normaId);
            var grupeFolosite = await _normaRepository.GetNumarGrupeOcupatePentruDisciplinaAsync(disciplinaId, norma.StatDefunctieId);

            int grupeRamase = Math.Max(grupeTotale - grupeFolosite, 0);
            System.Console.WriteLine("Numar grupe ramase: " + grupeRamase);
            ViewBag.GrupeDisponibile = Enumerable.Range(1, grupeRamase)
            .Select(i => new SelectListItem
            {
                Value = i.ToString(),
                Text = $"{i} grupe"
            }).ToList();

            var model = new AsociereDisciplinaNormaVacantaViewModel
            {
                NormaId = normaId,
                DisciplinaId = disciplinaId,
                StatDeFunctieId = norma.StatDefunctieId,
            };

            return View("~/Views/NormaVacanta/AsociazaDisciplinaNormaVacanta.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AsociazaDisciplinaNormaVacanta(AsociereDisciplinaNormaVacantaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var denumireDisciplina = await _disciplinaRepository.GetByIdAsync(model.DisciplinaId);
                ViewBag.DenumireDisciplina = denumireDisciplina?.DenumireDisciplina ?? "N/A";
                return View(model);
            }

            var nd_DeAdaugat = new NormaDisciplina
            {
                NormaId = model.NormaId,
                DisciplinaId = model.DisciplinaId,
                NumarSerii = model.NumarSerii,
                NumarGrupe = model.NumarGrupe,
                ActivitateTeoretica = false,
                ActivitatePractica = false,

            };

            await _normaDisciplinaRepository.AddAsync(nd_DeAdaugat);

            return RedirectToAction("EditareNormaVacanta", new {model.NormaId, ViewBag.StatDeFunctieId});
        }
        
        // [HttpPost]
        // public async Task<IActionResult> AdaugaDisciplina(int cadruDidacticId, int normaDisciplinaId, int statDeFunctieId)
        // {
        //     await _normaRepository.AsociazaDisciplinaLaCadruAsync(cadruDidacticId, normaDisciplinaId);
        //     return RedirectToAction("Edit", new { cadruDidacticId, statDeFunctieId });
        // }

    }
}
