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
    public class IncidenciasComedorController : Controller
    {
        private readonly IRepositorioIncidenciasComedor iComedor;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasComedorController(IRepositorioIncidenciasComedor iiComedor, IRepositorioUsuarios iVUsuario, IHostingEnvironment environment)
        {
            this.iComedor = iiComedor ?? throw new ArgumentNullException(nameof(iiComedor));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }
        [Route("/comedor/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasComedor([FromBody] IncidenciasComedor incidencia)
         {
            int insert = await iComedor.IncidenciasComedor(incidencia);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/comedor/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasComedor incidencia)
        {
            int update = await iComedor.ActualizaIncidencia(incidencia);
            if (update != -1)
            {
                return Ok(update);
            }
            return BadRequest();
        }

        [Route("/comedor/incidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> getCedulasComedor(int id, int pregunta)
        {
            List<IncidenciasComedor> inci = await iComedor.GetIncidenciasPregunta(id, pregunta);
            if (inci != null)
            {
                return Ok(inci);
            }
            return BadRequest();
        }

        [Route("/comedor/tablaIncidencias/{id?}/{pregunta?}/{pusuario}")]
        public async Task<IActionResult> generaTablaincidencias(int id, int pregunta,int pusuario)
        {
            string theadp1 = "<thead><tr><th>#</th><th>Tipo</th>" + (pregunta == 23 ? "<th>Incumplimiento</th>" : "") + "<th>Fecha de incidencia</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp2 = "<thead><tr><th>#</th><th>Tipo</th><th>Horario de apertura</th><th>Horario real</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp3 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha de toma de muestra</th><th class='text-center'>Muestras fuera de rango</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp4 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha programada</th><th>Fecha Realizada</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp5 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha de toma de muestra Testigo</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp6 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha de inventario</th><th>Fecha de entrega</th><th>Cantidad de equipo de cocina y<br/> enseres de mesa faltantes</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp7 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha en la que no se tomaron las<br/> muestras testigo completas</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string theadp8 = "<thead><tr><th>#</th><th>Tipo</th><th>Fecha de inventario</th>"+(pregunta == 13 ? "<th>Fecha de notificación</th>": "<th>Fecha acordada con la administración del inmueble</th>") + "<th>Fecha de entrega</th><th>Cantidad de equipo de cocina y enseres de mesa faltantes</th><th>Observaciones</th><th>Acciones</th></tr></thead><tbody>";
            string tbody = "";
            string table = "";
            List<IncidenciasComedor> incidencias = await iComedor.GetIncidenciasPregunta(id, pregunta);
            if (incidencias != null)
            {
                int i = 0;
                foreach (var inc in incidencias)
                {
                    i++;
                    if (pregunta == 2 || pregunta == 7 || pregunta == 8 || pregunta == 17 || pregunta == 18 || pregunta == 16
                        || pregunta == 19 || pregunta == 20 || pregunta == 21 || pregunta == 22 || pregunta == 23)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i  + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                (pregunta == 23 ? "<td>" + inc.Incumplimiento + "</td>":"") +
                                "<td>" + inc.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios":inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-fechaincidencia='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' data-comentarios='" + inc.Comentarios + "' " +
                                    "data-incumplimiento='" + inc.Incumplimiento + "'" +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-pusuario='" + pusuario + "' data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 15)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i  + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                "<td>" + inc.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios":inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-fechaincidencia='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' data-comentarios='" + inc.Comentarios + "' " +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-pusuario='" + pusuario + "'data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 5 || pregunta == 6)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                "<td>" + inc.FechaProgramada.ToString("dd/MM/yyyy HH:mm tt") + "</td>" +
                                "<td>" + inc.FechaEntrega.ToString("dd/MM/yyyy HH:mm tt") + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios" : inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    "data-fechaprogramada='" + inc.FechaProgramada.ToString("yyyy-MM-ddTHH:mm") + "' data-fechaentrega='" + inc.FechaEntrega.ToString("yyyy-MM-ddTHH:mm") +"'"+
                                    " data-comentarios='" + inc.Comentarios + "' " +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    "data-pusuario='" + pusuario + "' data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 9)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                "<td>" + inc.FechaProgramada.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + inc.FechaEntrega.ToString("dd/MM/yyyy") + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios" : inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-pusuario='" + pusuario + "' data-fechaprogramada='" + inc.FechaProgramada.ToString("yyyy-MM-dd") + "' data-fechaentrega='" + inc.FechaEntrega.ToString("yyyy-MM-dd") + "'" +
                                    "data-comentarios='" + inc.Comentarios + "' " +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    "data-pusuario='" + pusuario + "' data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if(pregunta == 12 || pregunta == 13 || pregunta == 14)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                "<td class='text-center'>" + inc.FechaProgramada.ToString("dd/MM/yyyy") + "</td>" +
                                (pregunta == 13 || pregunta == 14 ?"<td class='text-center'>" + 
                                    inc.FechaIncidencia.ToString("dd/MM/yyyy")
                                    + "</td>":"") +
                                "<td class='text-center'>" + inc.FechaEntrega.ToString("dd/MM/yyyy") + "</td>" +
                                "<td class='text-center'>" + inc.TotalMuestras + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios" : inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-cumplio='" + (inc.Cumplio == true ? 1:0) + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-pusuario='" + pusuario + "' data-fechaprogramada='" + inc.FechaProgramada.ToString("yyyy-MM-dd") + "' data-fechaentrega='" + inc.FechaEntrega.ToString("yyyy-MM-dd") + "'" +
                                    " data-fechaincidencia='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' data-comentarios='" + inc.Comentarios + "' data-muestras='" + inc.TotalMuestras + "'" +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    "data-pusuario='" + pusuario + "' data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                    else if (pregunta == 10 || pregunta == 11)
                    {
                        tbody +=
                            "<tr>" +
                                "<td>" + i + "</td>" +
                                "<td>" + inc.TipoIncidencia + "</td>" +
                                "<td class='text-center'>" + inc.FechaIncidencia.ToString("dd/MM/yyyy") + "</td>" +
                                "<td class='text-center'>" + inc.TotalMuestras + "</td>" +
                                "<td>" + (inc.Comentarios.Equals("") ? "Sin Comentarios" : inc.Comentarios) + "</td>" +
                                "<td>" +
                                    "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    " data-pusuario='" + pusuario + "' data-fechaincidencia='" + inc.FechaIncidencia.ToString("yyyy-MM-dd") + "' "+
                                    " data-muestras='" + inc.TotalMuestras + "' data-comentarios='" + inc.Comentarios + "' " +
                                    "data-toggle='tooltip' title='Actualizar Incidencia' data-placement='top'>" +
                                        "<i class='fas fa-edit text-primary'></i>" +
                                    "</a>" +
                                    "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "' data-pregunta='" + inc.Pregunta + "'" +
                                    "data-pusuario='" + pusuario + "' data-toggle='tooltip' title='Eliminar Incidencia' data-placement='top'>" +
                                    "<i class='fas fa-times text-danger'></i></a>" +
                                "</td>" +
                            "</tr>";
                    }
                }
                tbody += "</tbody>";
                table = (pregunta == 2 || pregunta == 7 || pregunta == 8 || pregunta == 17 || pregunta == 18 || pregunta == 16
                        || pregunta == 19 || pregunta == 20 || pregunta == 21 || pregunta == 22 || pregunta == 23) ? (theadp1 + tbody) : (pregunta == 5 || pregunta == 6) ? (theadp2 + tbody) :
                        (pregunta == 10 || pregunta == 11) ? (theadp3 + tbody): (pregunta == 9) ? (theadp4+tbody): 
                        (pregunta == 12) ? 
                        (theadp6+tbody): (pregunta == 13 || pregunta == 14) ? (theadp8 + tbody) : (pregunta == 15) ? (theadp7 + tbody) : (theadp5 + tbody);
                return Ok(table);
            }
            return BadRequest();
        }

        [Route("/comedor/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int excel = await iComedor.EliminaIncidencia(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/comedor/eliminaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, int pregunta)
        {
            int excel = await iComedor.EliminaTodaIncidencia(id, pregunta);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        [Route("/comedor/totalIncidencia/{id?}/{pregunta?}")]
        public async Task<IActionResult> IncidenciasTipo(int id, int pregunta)
        {
            int total = ((List<IncidenciasComedor>)await iComedor.GetIncidenciasPregunta(id, pregunta)).Count;
            if (total != -1)
            {
                return Ok(total);
            }
            return BadRequest();
        }

    }
}
