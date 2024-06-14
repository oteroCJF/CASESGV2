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
    public class IncidenciasTrasladoController : Controller
    {
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IHostingEnvironment environment;

        public IncidenciasTrasladoController(IRepositorioIncidenciasTraslado ivTraslado, IHostingEnvironment environment)
        {
            this.iTraslado = ivTraslado ?? throw new ArgumentNullException(nameof(ivTraslado));
        }

        [Route("/trasladoExp/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasTraslado([FromBody] IncidenciasTraslado incidenciasTraslado)
        {
            int success = await iTraslado.InsertaIncidencia(incidenciasTraslado);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasTraslado incidenciasTraslado)
        {
            int success = await iTraslado.ActualizaIncidencia(incidenciasTraslado);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidencias")]
        public async Task<IActionResult> getIncidencias(int id)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidencias(id);
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidenciasTable/{id?}/{pregunta?}")]
        public async Task<IActionResult> getIncidenciasTable(int id, int pregunta)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidenciasByPregunta(id,pregunta);
            string table = "";
            if (pregunta == 1)
            {
                table = "<thead>" +
                                  "<tr>" +
                                      "<th>ID</th>" +
                                      "<th>Fecha Incumplimiento</th>" +
                                      "<th>Personal Solicitado</th>" +
                                      "<th>Personal Brindado</th>" +
                                      "<th>Comentarios</th>" +
                                      "<th>Acciones</th>" +
                                  "</tr>" +
                              "</thead>" +
                              "<tbody></tbody>";
            } else {
                table = "<thead>" +
                                "<tr>" +
                                    "<th>ID</th>" +
                                    "<th>Fecha Incumplimiento</th>" +
                                    "<th>Incumplimiento</th>" +
                                    "<th>Comentarios</th>" +
                                    "<th>Acciones</th>" +
                                "</tr>" +
                            "</thead>" +
                            "<tbody></tbody>";
            }
            string body = "";
            string coments = "El prestador incumplió con lo siguiente: <br/>";
            int i = 0;
            if (incidencias != null)
            {
                foreach (var res in incidencias)
                {
                    if (pregunta == 3) {
                        var com = res.IncidenciasEquipo.Split("|");
                        for (var m = 0; m < com.Length-1; m++)
                        {
                            coments += "<li>"+com[m] + "</li>";
                        }

                        i++;
                        body +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + res.FechaIncumplida.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + coments + "</td>" +
                                "<td>" + res.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-primary mr-3 update_incumplimiento' data-fecha='" + res.FechaIncumplida.ToString("yyyy-MM-dd") + "' " +
                                        "data-id='" + res.Id + "' data-coments='" + res.Comentarios + "' data-incidencias='" + res.IncidenciasEquipo + "'><i class='fas fa-pencil'></i></a>" +
                                    "<a href='#' class='text-danger delete_incumplimiento' data-id='" + res.Id + "' data-pregunta='" + res.Pregunta + "'><i class='fas fa-times'></i></a>" +
                                "</td>" +
                            "</tr>";
                        coments = "";
                    }
                    else
                    {
                        i++;
                        body +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + res.FechaIncumplida.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + res.PersonalSolicitado + " persona(s)</td>" +
                                "<td>" + res.PersonalBrindado + " persona(s)</td>" +
                                "<td>" + res.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-primary mr-3 update_incumplimiento' data-fecha='" + res.FechaIncumplida.ToString("yyyy-MM-dd") + "' " +
                                    " data-pbrindado ='" + res.PersonalBrindado + "' data-psolicitado ='" + res.PersonalSolicitado + "' data-pregunta='" + res.Pregunta + "' " +
                                        "data-id='" + res.Id + "' data-coments='" + res.Comentarios + "'><i class='fas fa-pencil'></i></a>" +
                                    "<a href='#' class='text-danger delete_incumplimiento' data-id='" + res.Id + "' data-pregunta='" + res.Pregunta + "'><i class='fas fa-times'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                }
                return Ok(table += body);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iTraslado.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/trasladoExp/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int excel = await iTraslado.EliminaTodaIncidencia(id, pregunta);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasTraslado>)await iTraslado.getIncidenciasByPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }

    }
}
