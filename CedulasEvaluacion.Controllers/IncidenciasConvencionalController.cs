using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class IncidenciasConvencionalController : Controller
    {
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasConvencionalController(IRepositorioIncidenciasConvencional ivConvencional, IRepositorioUsuarios iVUsuario, IHostingEnvironment environment)
        {
            this.iConvencional = ivConvencional ?? throw new ArgumentNullException(nameof(ivConvencional));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }

        [Route("/telConvencional/inserta/incidencia")]
        public async Task<IActionResult> InsertaIncidencia([FromBody] IncidenciasConvencional incidenciasConvencional)
        {
            int excel = await iConvencional.InsertaIncidencia(incidenciasConvencional);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telConvencional/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasConvencional incidenciasConvencional)
        {
            int excel = await iConvencional.ActualizaIncidencia(incidenciasConvencional);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telConvencional/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iConvencional.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/telConvencional/eliminaIncidencias/{id?}/{tipo?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, string tipo)
        {
            int excel = await iConvencional.EliminaTodaIncidencia(id, tipo);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telConvencional/totalIncidencia/{id?}/{tipo?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, string tipo)
        {
            int total = await iConvencional.IncidenciasTipoConvencional(id, tipo);
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }


    }
}
