using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
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
    public class IncidenciasController : Controller
    {
        private readonly IRepositorioIncidencias iLimpieza;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;

        public IncidenciasController(IRepositorioIncidencias iiLimpieza, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario)
        {
            this.iLimpieza = iiLimpieza ?? throw new ArgumentNullException(nameof(iiLimpieza));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
        }


        /*Metodo que guarda las incidencias*/
        [HttpPost]
        [Route("/limpieza/insertar/incidencia")]
        public async Task<IActionResult> insertarIncidencia([FromBody] IncidenciasLimpieza incidencia)
        {
            int success = await iLimpieza.insertarIncidencia(incidencia);
            if (success != 0)
            {
                return Ok(success);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("/limpieza/actualizar/incidencia")]
        public async Task<IActionResult> actualizarIncidencia([FromBody] IncidenciasLimpieza incidencia)
        {
            int update = await iLimpieza.actualizarIncidencia(incidencia);
            if (update != 0)
            {
                return Ok(update);
            }
            return NoContent();
        }
        
        [HttpGet]
        [Route("/limpieza/getIncidencias/{id}/{pregunta}")]
        public async Task<IActionResult> getIncidenciasByPregunta(int id, int pregunta)
        {
            List<IncidenciasLimpieza> incidencias = await iLimpieza.getIncidenciasByPregunta(id,pregunta);
            string table = "";
            if (incidencias != null)
            {
                int i = 0;
                foreach (var tb in incidencias)
                {
                    i++;
                    table += 
                        "<tr>" +
                            "<td>" + (i) + "</td>" +
                            "<td>" + tb.Tipo + "</td>" +
                            "<td>" + tb.Area + "</td>" +
                            "<td>" + tb.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                            "<td>" + tb.Comentarios + "</td>" +
                            "<td>" +
                                "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + tb.Id + "'  data-pregunta ='" + tb.Pregunta + "' data-tipo='" + tb.Tipo+"' data-area='"+tb.Area+"'" +
                                " data-comentarios='"+tb.Comentarios+"' data-fechaincidencia='"+tb.FechaIncidencia.ToString("yyyy-MM-dd")+"'>" +
                                    "<i class='fas fa-edit text-primary'></i>" +
                                "</a>" +
                                "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + tb.Id + "'>" +
                                    "<i class='fas fa-times text-danger'></i>" +
                                "</a>" +
                            "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }

            return NoContent();
        }

        [Route("/limpieza/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int success = await iLimpieza.eliminarIncidencia(id);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/limpieza/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int delete = await iLimpieza.eliminarTodaIncidencia(id, pregunta);
            if (delete != -1)
            {
                return Ok(delete);
            }
            return BadRequest();
        }

        [Route("/limpieza/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasLimpieza>)await iLimpieza.getIncidenciasByPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }
    }
}
