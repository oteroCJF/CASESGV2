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
    public class IncidenciasTransporteController : Controller
    {
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IHostingEnvironment environment;

        public IncidenciasTransporteController(IRepositorioIncidenciasTransporte ivTransporte, IHostingEnvironment environment)
        {
            this.iTransporte = ivTransporte ?? throw new ArgumentNullException(nameof(ivTransporte));
            this.environment = environment;
        }

        [Route("/transporte/incidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> getCedulasTransporte(int id, int pregunta)
        {
            List<IncidenciasTransporte> inci = await iTransporte.GetIncidenciasPregunta(id, pregunta);
            if (inci != null)
            {
                return Ok(inci);
            }
            return BadRequest();
        }

        [Route("/transporte/tablaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> generaTablaincidencias(int id, int pregunta)
        {
            string table = "";
            List<IncidenciasTransporte> incidencias = await iTransporte.GetIncidenciasPregunta(id, pregunta);
            if (incidencias != null)
            {
                int i = 0, hora = 0;
                string h = "N/A";
                foreach (var inc in incidencias)
                {
                    i++;
                    if (inc.Pregunta.Equals(2 + ""))
                    {
                        h = inc.HoraPresentada+"";
                    }


                    table +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + inc.Tipo + "</td>" +
                                "<td>" + inc.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                                //"<td>" + h + "</td>" +
                                "<td>" + inc.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-hora='" + inc.HoraPresentada +"' " +
                                    " data-fechainci='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' data-coment='" + inc.Comentarios + "'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                }
                return Ok(table);
            }
            return BadRequest();
        }

        [Route("/transporte/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasTransporte([FromBody] IncidenciasTransporte incidenciasTransporte)
        {
            int insert = await iTransporte.IncidenciasTransporte(incidenciasTransporte);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/transporte/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasTransporte incidenciasTransporte)
        {
            int update = await iTransporte.ActualizaIncidencia(incidenciasTransporte);
            if (update != -1)
            {
                return Ok(update);
            }
            return BadRequest();
        }

        [Route("/transporte/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iTransporte.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/transporte/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int excel = await iTransporte.EliminaTodaIncidencia(id, pregunta);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/transporte/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasTransporte>)await iTransporte.GetIncidenciasPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }

    }
}
