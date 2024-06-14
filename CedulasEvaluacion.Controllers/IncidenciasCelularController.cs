
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
    public class IncidenciasCelularController : Controller
    {
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasCelularController(IRepositorioIncidenciasCelular ivCelular, IRepositorioUsuarios iVUsuario, IHostingEnvironment environment)
        {
            this.iCelular = ivCelular ?? throw new ArgumentNullException(nameof(ivCelular));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }

        [Route("/telCelular/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasCelular([FromBody] IncidenciasCelular incidenciasCelular)
        {
            int excel = await iCelular.IncidenciasCelular(incidenciasCelular);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telCelular/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasCelular incidenciasCelular)
        {
            int excel = await iCelular.ActualizaIncidencia(incidenciasCelular);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telCelular/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iCelular.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/telCelular/eliminaIncidencias/{id?}/{tipo?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, string tipo)
        {
            int excel = await iCelular.EliminaTodaIncidencia(id, tipo);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/telCelular/totalIncidencia/{id?}/{tipo?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, string tipo)
        {
            int total = await iCelular.IncidenciasTipoCelular(id, tipo);
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }

    }
}
