using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementulStatelorDeFunctii.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagementulStatelorDeFunctii.Controllers
{
    [Route("StateDeFunctii/[controller]")]
    public class StatDeFunctieController : Controller
    {
        private readonly IStatDeFunctieRepository _statRepo;

        public StatDeFunctieController(IStatDeFunctieRepository statRepo)
        {
            _statRepo = statRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.StatDeFunctieId = id;
            var model = await _statRepo.GetProfesoriCuDisciplineAsync(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListaState()
        {
            var state = await _statRepo.GetAllStateAsync();
            return View(state);
        }



    }



}