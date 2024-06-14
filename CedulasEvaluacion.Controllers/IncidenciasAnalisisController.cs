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
    public class IncidenciasAnalisisController : Controller
    {
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasAnalisisController(IRepositorioIncidenciasAnalisis ivAnalisis, IRepositorioUsuarios iVUsuario, IHostingEnvironment environment)
        {
            this.iAnalisis = ivAnalisis ?? throw new ArgumentNullException(nameof(ivAnalisis));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }

        [Route("/analisis/incidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> getCedulasAnalisis(int id, int pregunta)
        {
            List<IncidenciasAnalisis> inci = await iAnalisis.GetIncidenciasPregunta(id, pregunta);
            if (inci != null)
            {
                return Ok(inci);
            }
            return BadRequest();
        }

        [Route("/analisis/tablaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> generaTablaincidencias(int id, int pregunta)
        {
            string tbody = "";
            List<IncidenciasAnalisis> incidencias = await iAnalisis.GetIncidenciasPregunta(id, pregunta);
            if (incidencias != null)
            {
                int i = 0;
                foreach (var inc in incidencias)
                {
                    tbody +=
                            "<tr>" +
                                "<td>" + (i + 1) + "</td>" +
                                "<td>" + inc.Tipo + "</td>" +
                                "<td>" + inc.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-fechainci='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' " +
                                    "data-coment='" + inc.Comentarios + "'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                }
                return Ok(tbody);
            }
            return BadRequest();
        }

        [Route("/analisis/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasAnalisis([FromBody] IncidenciasAnalisis incidenciasAnalisis)
        {
            int insert = await iAnalisis.IncidenciasAnalisis(incidenciasAnalisis);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/analisis/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasAnalisis incidenciasAnalisis)
        {
            int update = await iAnalisis.ActualizaIncidencia(incidenciasAnalisis);
            if (update != -1)
            {
                return Ok(update);
            }
            return BadRequest();
        }

        [Route("/analisis/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iAnalisis.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/analisis/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int excel = await iAnalisis.EliminaTodaIncidencia(id, pregunta);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/analisis/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasAnalisis>)await iAnalisis.GetIncidenciasPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }
    }
}
