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
    public class IncidenciasAguaController : Controller
    {
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasAguaController(IRepositorioIncidenciasAgua ivAgua, IRepositorioUsuarios iVUsuario, IHostingEnvironment environment)
        {
            this.iAgua = ivAgua ?? throw new ArgumentNullException(nameof(ivAgua));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }

        [Route("/agua/incidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> getCedulasFumigacion(int id, int pregunta)
        {
            List<IncidenciasAgua> inci = await iAgua.GetIncidenciasPregunta(id, pregunta);
            if (inci != null)
            {
                return Ok(inci);
            }
            return BadRequest();
        }

        [Route("/agua/tablaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> generaTablaincidencias(int id, int pregunta)
        {
            string theadp2 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha Programada</th><th>Fecha Realizada</th><th>Garrafones</th><th>Comentarios</th><th>Acciones</th></tr></thead><tbody>";
            string theadp3 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha Programada</th><th>Fecha Realizada</th><th>Comentarios</th><th>Acciones</th></tr></thead><tbody>";
            string theadp4 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha Programada</th><th>Fecha Realizada</th><th>Garrafones</th><th>Comentarios</th><th>Acciones</th></tr></thead><tbody>";
            string tbody = "";
            string table = "";
            List<IncidenciasAgua> incidencias = await iAgua.GetIncidenciasPregunta(id, pregunta);
            if (incidencias != null)
            {
                int i = 0;
                foreach (var inc in incidencias)
                {
                    if (pregunta == 2)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + (i + 1) + "</td>" +
                                "<td>" + inc.Tipo + "</td>" +
                                "<td>" + inc.FechaProgramada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.FechaRealizada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.Garrafones + "</td>" +
                                "<td>" + inc.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-fechareal='" + inc.FechaRealizada.ToString("yyyy-MM-dd") + "'" +
                                    " data-fechaprog='" + inc.FechaProgramada.ToString("yyyy-MM-dd") + "' data-coment='" + inc.Comentarios + "' data-garrafones='" + inc.Garrafones + "'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 4)
                    {
                        tbody +=
                           "<tr>" +
                                "<td>" + (i + 1) + "</td>" +
                                "<td>" + inc.Tipo + "</td>" +
                                "<td>" + inc.FechaProgramada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.FechaRealizada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.Comentarios + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-fechareal='" + inc.FechaRealizada.ToString("yyyy-MM-dd") + "'" +
                                    " data-fechaprog='" + inc.FechaProgramada.ToString("yyyy-MM-dd") + "' data-coment='" + inc.Comentarios + "' data-garrafones='" + inc.Garrafones + "'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 3)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + (i + 1) + "</td>" +
                                "<td>" + inc.Tipo + "</td>" +
                                "<td>" + inc.FechaProgramada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.FechaRealizada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.Garrafones + "</td>" +
                                "<td>" + inc.Comentarios.Replace("|", "<br>") + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-fechareal='" + inc.FechaRealizada.ToString("yyyy-MM-dd") + "'" +
                                    " data-fechaprog='" + inc.FechaProgramada.ToString("yyyy-MM-dd") + "' data-coment='" + inc.Comentarios + "' data-garrafones='" + inc.Garrafones + "'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                }
                tbody += "</tbody>";
                table = pregunta == 2 ? (theadp2 + tbody) : pregunta == 3 ? (theadp4 + tbody) : (theadp3 + tbody);
                return Ok(table);
            }
            return BadRequest();
        }


        [Route("/agua/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasAgua([FromBody] IncidenciasAgua incidenciasAgua)
        {
            int insert = await iAgua.IncidenciasAgua(incidenciasAgua);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/agua/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasAgua incidenciasAgua)
        {
            int update = await iAgua.ActualizaIncidencia(incidenciasAgua);
            if (update != -1)
            {
                return Ok(update);
            }
            return BadRequest();
        }

        [Route("/agua/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iAgua.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/agua/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int excel = await iAgua.EliminaTodaIncidencia(id, pregunta);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/agua/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasAgua>) await iAgua.GetIncidenciasPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }
    }
}
