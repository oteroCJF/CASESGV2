using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class IncidenciasMensajeriaController : Controller
    {
        private readonly IRepositorioIncidenciasMensajeria vIMensajeria;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasMensajeriaController(IRepositorioIncidenciasMensajeria iiMensajeria, IRepositorioFacturas iFacturas, 
                                               IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                               IHostingEnvironment environment)
        {
            this.vIMensajeria = iiMensajeria ?? throw new ArgumentNullException(nameof(iiMensajeria));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }

        
        [Route("/mensajeria/incidencias/subirExcel")]
        public async Task<IActionResult> IncidenciasExcel([FromForm] IncidenciasMensajeria incidenciasMensajeria)
        {
            int excel = await vIMensajeria.IncidenciasExcel(incidenciasMensajeria);
            if (excel == 1)
            {
                return Ok(excel);
            }
            return BadRequest(excel);
        }

        [Route("/mensajeria/incidencias")]
        public async Task<IActionResult> IncidenciasMensajeria([FromBody] IncidenciasMensajeria incidenciasMensajeria)
        {
            int excel = await vIMensajeria.IncidenciasMensajeria(incidenciasMensajeria);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/mensajeria/incidencias/robadas")]
        public async Task<IActionResult> IncidenciasMensajeriaRobadas([FromForm] IncidenciasMensajeria incidenciasMensajeria)
        {
            int excel = await vIMensajeria.IncidenciaRobada(incidenciasMensajeria);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/mensajeria/actualiza/robadas")]
        public async Task<IActionResult> ActualizaMensajeriaRobadas([FromForm] IncidenciasMensajeria incidenciasMensajeria)
        {
            int update = await vIMensajeria.ActualizaRobada(incidenciasMensajeria);
            if (update != -1)
            {
                return Ok(update);
            }
            return BadRequest();
        }

        [Route("/mensajeria/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasMensajeria incidenciasMensajeria)
        {
            int excel = await vIMensajeria.ActualizaIncidencia(incidenciasMensajeria);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }
        
        [Route("/mensajeria/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await vIMensajeria.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }
        
        [Route("/mensajeria/totalIncidencia/{id?}")]
        public async Task<IActionResult> TotalIncidencia(int id)
        {
            List<IncidenciasMensajeria> totales = await vIMensajeria.TotalIncidencias(id);
            if (totales != null)
            {
                return Ok(totales);
            }
            return BadRequest();
        }
        
        [Route("/mensajeria/totalIncidencia/{id?}/{pregunta}")]
        public async Task<IActionResult> TotalIncidenciasByPregunta(int id,int pregunta)
        {
            List<IncidenciasMensajeria> totales = await vIMensajeria.TotalIncidenciasByPregunta(id,pregunta);
            if (totales != null)
            {
                return Ok(totales.Count);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/mensajeria/eliminaIncidencias/{id?}/{tipo?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, string tipo)
        {
            int excel = await vIMensajeria.EliminaTodaIncidencia(id, tipo);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Incidencias por Tipo*/
        [Route("/mensajeria/incidenciasTipo/{id?}/{tipo?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, string tipo)
        {
            int total = await vIMensajeria.IncidenciasTipo(id, tipo);
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/mensajeria/getPlantilla")]
        public IActionResult getPlantilla()
        {
            string fileName = @"e:\Plantillas CASESGV2\DocsV2\Plantilla Incidencias.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Plantilla_Incidencias.xlsx");
        }
    }
}
