using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.DataLayer.ReposInterfaces;
using ManagementulStatelorDeFunctii.Models;
using ManagementulStatelorDeFunctii.ViewModels.StatDeFunctieViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementulStatelorDeFunctii.Controllers
{
    public class StatDeFunctieController : Controller
    {
        private readonly IStatDeFunctieRepository _statRepo;
        private readonly IGenericRepository<Departament> _departamentRepository;

        public StatDeFunctieController(
            IStatDeFunctieRepository statRepo,
            IGenericRepository<Departament> departamentRepository)
        {
            _statRepo = statRepo;
            _departamentRepository = departamentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.StatDeFunctieId = id;
            var model = await _statRepo.GetProfesoriCuDisciplineAsync(id);
            var stat = await _statRepo.GetById(id);
            ViewBag.AnStat = stat.An;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListaState()
        {
            var state = await _statRepo.GetAllStateAsync();
            return View(state);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDraft(int id)
        {
            var stat = await _statRepo.GetById(id);
            if (stat == null)
                return NotFound();

            stat.Draft = !stat.Draft;
            await _statRepo.SaveChangesAsync();

            TempData["Mesaj"] = stat.Draft
            ? "Statul a fost marcat ca *Draft*."
            : "Statul a fost finalizat cu succes.";

            return RedirectToAction("ListaState");
        }

        [HttpGet]
        public async Task<IActionResult> CreeazaStatDeFunctie()
        {
            var departamente = (await _departamentRepository.GetAllAsync())
                .Select(d => new SelectListItem
                {
                    Value = d.DepartamentId.ToString(),
                    Text = d.DenumireDepartament
                }).ToList();

            var viewModel = new CreeazaStatDeFunctieViewModel
            {
                Departamente = departamente,
                AnStat = DateTime.Now.Year
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreeazaStatDeFunctie(CreeazaStatDeFunctieViewModel model)
        {
            var stat = new StatDeFunctie
            {
                DepartamentId = model.DepartamentId,
                An = model.AnStat,
                Draft = true
            };

            await _statRepo.AddAsync(stat);
            return RedirectToAction("ListaState");
        }

        [HttpPost]
        public async Task<IActionResult> StergeStatDeFunctie(int id)
        {
            await _statRepo.StergeStatDeFunctie(id);
            return RedirectToAction("ListaState");
        }



    }



}