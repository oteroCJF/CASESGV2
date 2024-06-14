using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class CompilacionController : Controller
    {
        [Route("/compilacion/index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
