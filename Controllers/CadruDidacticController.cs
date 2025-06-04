using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.Controllers
{
    public class CadruDidacticController : Controller
    {
        private readonly IGenericRepository<CadruDidactic> _repoCadru;
        private readonly IGenericRepository<GradDidactic> _repoGrade;
        private readonly IGenericRepository<Departament> _repoDepartament;
        private readonly IGenericRepository<CadruDidacticGradDidactic> _repoCadruDidacticGradDidactic;
        private readonly IGenericRepository<StatDeFunctie> _repoStat;
        private readonly ICadruDidacticStatFunctieTarifPlataOraRepository _repoTarif;
        private readonly ICadruDidacticRepository _cadruDidacticRepository;
        public CadruDidacticController(
            IGenericRepository<CadruDidactic> repoCadru,
            IGenericRepository<GradDidactic> repoGrade,
            IGenericRepository<Departament> repoDepartament,
            IGenericRepository<CadruDidacticGradDidactic> repoCadruDidacticGradDidactic,
            IGenericRepository<StatDeFunctie> repoStat,
            ICadruDidacticStatFunctieTarifPlataOraRepository repoTarif,
            ICadruDidacticRepository cadruDidacticRepository)
        {
            _repoCadru = repoCadru;
            _repoGrade = repoGrade;
            _repoDepartament = repoDepartament;
            _repoCadruDidacticGradDidactic = repoCadruDidacticGradDidactic;
            _repoTarif = repoTarif;
            _repoStat = repoStat;
            _cadruDidacticRepository = cadruDidacticRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cadre = await  _cadruDidacticRepository.GetCadreDidacticeCompletAsync();
            return View(cadre);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CadruDidacticFormViewModel
            {
                CadruDidactic = new CadruDidactic(),
                GradeDisponibile = (await _repoGrade.GetAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.GradDidacticId.ToString(),
                        Text = g.NumeGrad
                    }),
                DepartamenteDisponibile = (await _repoDepartament.GetAllAsync())
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartamentId.ToString(),
                        Text = d.DenumireDepartament
                    })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CadruDidacticFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Any())
                    {
                        Console.WriteLine($"Eroare la câmpul '{entry.Key}':");
                        foreach (var error in entry.Value.Errors)
                        {
                            Console.WriteLine($"    - {error.ErrorMessage}");
                        }
                    }
                }
                viewModel.GradeDisponibile = (await _repoGrade.GetAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.GradDidacticId.ToString(),
                        Text = g.NumeGrad
                    });

                viewModel.DepartamenteDisponibile = (await _repoDepartament.GetAllAsync())
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartamentId.ToString(),
                        Text = d.DenumireDepartament
                    });
                    
                return View(viewModel);
            }

            // var cadruDidacticGradDidactic = new CadruDidacticGradDidactic
            // {
            //     CadruDidacticId = viewModel.CadruDidactic.CadruDidacticId,
            //     GradDidacticId = viewModel.CadruDidactic.GradDidacticId
            // };

            // await _repoCadruDidacticGradDidactic.AddAsync(cadruDidacticGradDidactic);


            await _repoCadru.AddAsync(viewModel.CadruDidactic);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AsociazaCadruStat()
        {
            var model = new StatFunctieCadruViewModel
            {
                CadreDisponibile = (await _repoCadru.GetAllAsync()).Select(c => new SelectListItem
                {
                    Value = c.CadruDidacticId.ToString(),
                    Text = c.Nume
                }),
                StateDisponibile = (await _repoStat.GetAllAsync()).Select(s => new SelectListItem
                {
                    Value = s.StatDeFunctieId.ToString(),
                    Text = s.An.ToString()
                }),
                GradeDisponibile = (await _repoGrade.GetAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.GradDidacticId.ToString(),
                        Text = g.NumeGrad
                    }),
                DataDeLa = DateTime.Now.Year
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AsociazaCadruStat(StatFunctieCadruViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // model.CadreDisponibile = (await _repoCadru.GetAllAsync()).Select(...);
                // model.StateDisponibile = (await _repoStat.GetAllAsync()).Select(...);
                await ReincarcaDropdownuri(model);
                return View(model);
            }
            if (await _repoTarif.ExistaAsociereAsync(model.StatDeFunctieId, model.CadruDidacticId))
            {
                ModelState.AddModelError("", "Acest cadru didactic este deja asociat cu statul selectat având același grad.");
                await ReincarcaDropdownuri(model);
                return View(model);
            }



            var cadruDidacticGradDidactic = new CadruDidacticGradDidactic
            {
                CadruDidacticId = model.CadruDidacticId,
                GradDidacticId = model.GradDidacticId,
                DeLa = model.DataDeLa,
                PanaLa = model.DataPanaLa
            };

            await _repoCadruDidacticGradDidactic.AddAsync(cadruDidacticGradDidactic);

            var cadruDidacticTarif = new CadruDidacticStatFunctieTarifPlataOra
            {
                CadruDidacticId = cadruDidacticGradDidactic.CadruDidacticGradDidacticId,
                StatFunctieId = model.StatDeFunctieId,
                Tarif = (int)model.Tarif
            };
            await _repoTarif.AddAsync(cadruDidacticTarif);

            return RedirectToAction("Index", "CadruDidactic");
        }

        private async Task ReincarcaDropdownuri(StatFunctieCadruViewModel model)
        {
            model.CadreDisponibile = (await _repoCadru.GetAllAsync()).Select(c => new SelectListItem
            {
                Value = c.CadruDidacticId.ToString(),
                Text = c.Nume
            });

            model.StateDisponibile = (await _repoStat.GetAllAsync()).Select(s => new SelectListItem
            {
                Value = s.StatDeFunctieId.ToString(),
                Text = s.An.ToString()
            });

            model.GradeDisponibile = (await _repoGrade.GetAllAsync()).Select(g => new SelectListItem
            {
                Value = g.GradDidacticId.ToString(),
                Text = g.NumeGrad
            });
        }

        
        [HttpGet]
        public async Task<IActionResult> EditCadru(int id)
        {
            var cadru = await _repoCadru.GetByIdAsync(id);
            if (cadru == null)
                return NotFound();

            var grade = await _repoGrade.GetAllAsync();
            var departamente = await _repoDepartament.GetAllAsync();

            var viewModel = new CadruDidacticFormViewModel
            {
                CadruDidactic = cadru,
                GradeDisponibile = grade.Select(g => new SelectListItem
                {
                    Value = g.GradDidacticId.ToString(),
                    Text = g.NumeGrad,
                    Selected = g.GradDidacticId == cadru.GradDidacticId
                }),
                DepartamenteDisponibile = departamente.Select(d => new SelectListItem
                {
                    Value = d.DepartamentId.ToString(),
                    Text = d.DenumireDepartament,
                    Selected = d.DepartamentId == cadru.DepartamentId
                })
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCadru(int id, CadruDidacticFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var grade = await _repoGrade.GetAllAsync();
                var departamente = await _repoDepartament.GetAllAsync();

                model.GradeDisponibile = grade.Select(g => new SelectListItem
                {
                    Value = g.GradDidacticId.ToString(),
                    Text = g.NumeGrad
                });

                model.DepartamenteDisponibile = departamente.Select(d => new SelectListItem
                {
                    Value = d.DepartamentId.ToString(),
                    Text = d.DenumireDepartament
                });

                return View(model);
            }

            var cadruDinDb = await _repoCadru.GetByIdAsync(id);
            if (cadruDinDb == null)
                return NotFound();

            if (cadruDinDb.GradDidacticId != model.CadruDidactic.GradDidacticId)
            {
                var toateRelatiile = await _repoCadruDidacticGradDidactic.GetAllAsync();
                var relatieActiva = toateRelatiile
                    .FirstOrDefault(r =>
                        r.CadruDidacticId == id && r.PanaLa == null);

                if (relatieActiva != null)
                {
                    relatieActiva.PanaLa = DateTime.Now.Year;
                    await _repoCadruDidacticGradDidactic.UpdateAsync(relatieActiva);
                }

                var relatieNoua = new CadruDidacticGradDidactic
                {
                    CadruDidacticId = id,
                    GradDidacticId = model.CadruDidactic.GradDidacticId,
                    DeLa = DateTime.Now.Year,
                    PanaLa = null
                };

                await _repoCadruDidacticGradDidactic.AddAsync(relatieNoua);
            }

            cadruDinDb.Nume = model.CadruDidactic.Nume;
            cadruDinDb.GradDidacticId = model.CadruDidactic.GradDidacticId;
            cadruDinDb.DepartamentId = model.CadruDidactic.DepartamentId;

            await _repoCadru.UpdateAsync(cadruDinDb);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCadru(int id)
        {
            var cadru = await _repoCadru.GetByIdAsync(id);
            if (cadru == null)
                return NotFound();

            await _repoCadru.DeleteAsync(id);
            return RedirectToAction("Index");
        }



    }
}