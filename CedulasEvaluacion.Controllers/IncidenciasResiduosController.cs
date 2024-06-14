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
    public class IncidenciasResiduosController : Controller
    {
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IHostingEnvironment environment;

        private string[] manifiestoEntrega = { "Nombre, sello y firma del titular del servicio médico.", "Domicilio del consultorio donde se brinda el servicio.", "Descripción e identificación del tipo de residuo.",
                                               "Nombre y firma del responsable de la recolección.", "Cantidad y unidad de medida del residuo.", "Envase o contenedor utilizado.", "Instrucciones especiales e información para su manejo seguro.",
                                               "Descripción general de la transportación y del centro de acopio o destinatario.", "Número de registro ambiental.", "Fecha y número del manifiesto.", "Clasificación." };
        private string[] manifiestoEntregaBD = { "DatosTitularMedico", "ConsultorioBrindaServicio", "DescripcionResiduo", "DatosRecoleccion", "DetallesResiduo", "EnvaseUtilizado", "ManejoSeguro",
                                                "CentroDestinatario","RegistroAmbiental", "NumeroManifiesto", "Clasificacion" };

        public IncidenciasResiduosController(IRepositorioIncidenciasResiduos ivResiduos, IHostingEnvironment environment)
        {
            this.iResiduos = ivResiduos ?? throw new ArgumentNullException(nameof(ivResiduos));
        }

        [Route("/residuos/incidencia")]
        public async Task<IActionResult> insertaIncidencia([FromBody] IncidenciasResiduos incidenciasResiduos)
        {
            int success = await iResiduos.insertaIncidencia(incidenciasResiduos);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/residuos/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasResiduos incidenciasResiduos)
        {
            int success = await iResiduos.ActualizaIncidencia(incidenciasResiduos);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/residuos/getIncidencias/{id?}/{tipo?}")]
        public async Task<IActionResult> getIncidenciasTipo(int id, string tipo)
        {
            List<IncidenciasResiduos> incidencias = await iResiduos.getIncidenciasTipo(id, tipo);
            string rpbi = "";
            if (tipo.Equals("ManifiestoEntrega") && incidencias.Count != 0)
            {
                var com = incidencias[0].Comentarios.Split("|");
                for (var i = 0; i < com.Length; i++)
                {
                    for (var j = 0; j < manifiestoEntregaBD.Length; j++)
                    {
                        if (manifiestoEntrega[j].Equals(com[i]))
                        {
                            rpbi += manifiestoEntregaBD[j] + "|";
                            break;
                        }
                    }
                }
                incidencias[0].Comentarios = rpbi;
            }
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/residuos/getIncidenciasPregunta4/{id?}/{pregunta?}")]
        public async Task<IActionResult> getIncidenciasPregunta4(int id, string pregunta)
        {
            List<IncidenciasResiduos> incidencias = await iResiduos.getIncidenciasTipo(id, pregunta);
            string rpbi = "";
            string table = "";
            string coments = "";
            if (incidencias.Count != 0)
            {
                var com = incidencias[0].Comentarios.Split("|");
                for (var i = 0; i<com.Length; i++)
                {
                    for (var j = 0; j < manifiestoEntregaBD.Length; j++)
                    {
                        if (manifiestoEntrega[j].Equals(com[i]))
                        {
                            coments += manifiestoEntrega[j] + "<br />";
                            rpbi+= manifiestoEntregaBD[i]+"|";
                            break;
                        }
                    }
                }

                foreach (var res in incidencias)
                {
                    table +=
                        "<tr>" +
                            "<td> 1 </td>" +
                            "<td> Manifiesto Entrega </td>" +
                            "<td>" + coments + "</td>" +
                            "<td>" +
                                "<a href='#' class='text-primary mr-3 update_pregunta4' data-tipo ='" + res.Tipo + "'  data - id = '" + res.Id + "' "+
                                    "data-coment ='" + rpbi + "'><i class='fas fa-pencil'></i></a>" +
                                "<a href='#' class='text-danger delete_pregunta4' data-id='" + res.Id + "'><i class='fas fa-times'></i></a>" +
                            "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }
            else if(incidencias.Count == 0)
            {
                return Ok("");
            }

            return BadRequest();
        }

        [Route("/residuos/getIncidenciasPregunta5/{id?}")]
        public async Task<IActionResult> getIncidenciasPregunta5(int id)
        {
            List<IncidenciasResiduos> incidencias = await iResiduos.getIncidencias(id);
            string table = "";
            int i = 0;
            if (incidencias.Count != 0)
            {
                foreach (var res in incidencias)
                {
                    i++;
                    table +=
                        "<tr>" +
                            "<td>"+ i +"</td>" +
                            "<td>" + res.Tipo + "</td>" +
                            "<td>" + res.Comentarios + "</td>" +
                            "<td>" +
                                "<a href='#' class='text-primary mr-3 update_pregunta5' data-tipo ='" + res.Tipo + "'  data-id = '" + res.Id + "' " +
                                    "data-coment ='" + res.Comentarios + "'><i class='fas fa-pencil'></i></a>" +
                                "<a href='#' class='text-danger delete_pregunta5' data-id='" + res.Id + "'><i class='fas fa-times'></i></a>" +
                            "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }else if(incidencias.Count == 0)
            {
                return Ok(incidencias.Count);
            }

            return BadRequest();
        }

        [Route("/residuos/eliminaIncidencia/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int success = await iResiduos.EliminaIncidencias(id);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        /*Elimina todas las incidencias*/
        [Route("/residuos/eliminaIncidencias/{id?}/{tipo?}")]
        public async Task<IActionResult> EliminaTodaIncidencia(int id, string tipo)
        {
            int success = await iResiduos.EliminaTodaIncidencia(id, tipo);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/residuos/totalIncidencia/{id?}/{pregunta}")]
        public async Task<IActionResult> TotalIncidenciasByPregunta(int id, int pregunta)
        {
            List<IncidenciasResiduos> totales = await iResiduos.getIncidenciasByPregunta(id, pregunta);
            if (totales != null)
            {
                return Ok(totales.Count);
            }
            return BadRequest();
        }
    }
}
