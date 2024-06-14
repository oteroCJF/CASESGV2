using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class ErroresController : Controller
    {
        [HttpGet]
        [Route("/error/denied")]
        public ActionResult Acceso()
        {
            return View();
        }

        [HttpGet]
        [Route("/error/cedSend")]
        public ActionResult Enviada()
        {
            return View();
        }

        [HttpGet]
        [Route("/error/perfilNotFound")]
        public ActionResult AsignacionPerfil()
        {
            return View();
        }


        [HttpGet]
        [Route("/casesg")]
        public ActionResult Tiempo()
        {
            return View();
        }
    }
}
