using CedulasEvaluacion.Entities.MAlcances;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class AlcancesController : Controller
    {
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioAlcancesCedula vAlcances;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioVariables vVariables;

        public AlcancesController(IRepositorioAlcancesCedula iAlcances, IRepositorioCatalogoServicios iCatalogo, IRepositorioPerfiles iPerfiles,
            IRepositorioVariables iVariables)
        {
            this.vCatalogo = iCatalogo ?? throw new ArgumentNullException(nameof(iCatalogo));
            this.vAlcances = iAlcances ?? throw new ArgumentNullException(nameof(iAlcances));
            this.vVariables= iVariables?? throw new ArgumentNullException(nameof(iVariables));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        [Route("/alcances/index")]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "Servicio")] int servicio, [FromQuery(Name = "Anio")] int anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                MAlcances models = new MAlcances();
                models.catalogo = await vCatalogo.GetServiciosByUsers(UserId());
                if (servicio != 0)
                {
                    models.ServicioId= servicio;
                    models.anios = await vVariables.GetAniosEvaluacion();
                }

                if (servicio != 0 && anio != 0)
                {
                    models.ServicioId = servicio;
                    models.Anio = anio;
                    models.concentrado = await vAlcances.getCedulasByMes(servicio, anio);
                }
                return View(models);
            }

            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/alcances/habilitar")]
        public async Task<IActionResult> habilitarAlcances([FromBody] CedulaEvaluacion cedula)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "habilitar");
            if (success == 1)
            {
                int active = await vAlcances.habilitaAlcancesCedulas(cedula);
                if (success != 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Alcances";
        }
    }
}
