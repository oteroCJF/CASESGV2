using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ValidacionesController : Controller
    {
        private readonly IRepositorioValidaciones vValidaciones;

        public ValidacionesController(IRepositorioValidaciones iValidaciones)
        {
            this.vValidaciones = iValidaciones ?? throw new ArgumentNullException(nameof(iValidaciones));
        }

        [HttpPost]
        [Route("/valida/periodoComedor")]
        public async Task<IActionResult> GeneraCedulaComedor([FromBody] CedulaEvaluacion cedula)
        {
            bool permit = await vValidaciones.ObtienePeriodoComedor(cedula);
            if (permit)
            {
                return Ok(permit);
            }
            return Ok(permit);
        }
        
        [HttpPost]
        [Route("/valida/comedor/fechaIncidencia")]
        public async Task<IActionResult> GetFechaIncidenciaComedor([FromBody] IncidenciasComedor comedor)
        {
            bool permit = await vValidaciones.GetValidacionIncidenciaComedor(comedor);
            if (permit)
            {
                return Ok(permit);
            }
            return Ok(permit);
        }
    }
}
