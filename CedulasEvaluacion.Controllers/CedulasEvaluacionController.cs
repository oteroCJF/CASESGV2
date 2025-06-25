using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using CedulasEvaluacion.Interfaces;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System.Globalization;
using System.IO;
using Microsoft.Reporting.NETCore;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.MFirmantes;
using Microsoft.AspNetCore.Authorization;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class CedulasEvaluacionController : Controller
    {

        private string[] tiposIncidencia = { "Recoleccion", "Entrega", "Acuses", "Mal Estado", "Extraviadas", "Robadas",""};
        private string[] incidenciasAgua = {"", "Fechas", "Horas", "Numeral" };
        private string[] tiposCelular = {"","Alta_Equipo", "Alta", "Baja", "Reactivacion", "Suspension", "Perfil", "SIM", "CambioNumeroRegion",
                                          "VozDatos", "Diagnostico", "Reparacion"};
        private string[] rCelular = {"", "altas con entrega de equipo", "altas sin entrega de equipo", "bajas de servicio", "reactivaciones por robo/extravío", "suspensiones por robo/extravío", "cambios de perfil", "switcheos de SIM Card", "cambios de número o región",
                                          "solicitudes de voz y/o datos", "solicitudes de diagnóstico de equipo", "solicitudes de reparación de equipo"};
        private string[] tiposConv = { "", "contratacion_instalacion", "cableado", "entregaAparato", "cambioDomicilio", "reubicacion", "identificadorLlamadas", "troncales", "internet",
                                          "serviciosTelefonia", "cancelacion", "reportesFallas"};
        private string[] rConv = {"", "nuevas contrataciones", "cableados interiores para la instalación de líneas telefónicas","entregas de aparatos telefónicos", "cambios de domicilio",
            "reubicaciones de lineas telefónicas comerciales en el mismo inmueble", "activaciones para el identificador de llamadas",
            "instalaciones de troncales y DID's",
                                   "instalación de internet", "habilitación de servicios",
                                          "cancelación de servicios", "reportes de fallas"};


        private readonly IWebHostEnvironment web;

        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioCuestionarios vQuestion;
        private readonly IRepositorioReporteCedula vrCedula;
        private readonly IRepositorioIncidencias iLimpieza;
        private readonly IRepositorioIncidenciasComedor iComedor;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;
        private readonly IRepositorioIncidenciasMensajeria iMensajeria;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IRepositorioFirmantes vFirmantes;
        private readonly IRepositorioVariables vVariables;

        public CedulasEvaluacionController(IWebHostEnvironment vweb, IRepositorioReporteCedula vvReporte, IRepositorioIncidenciasMensajeria iiMensajeria,
            IRepositorioIncidencias iiLimpieza, IRepositorioEvaluacionServicios viCedula, IRepositorioIncidenciasFumigacion iiFumigacion, IRepositorioIncidenciasTraslado iiTraslado,
            IRepositorioIncidenciasMuebles IiMuebles, IRepositorioIncidenciasCelular iiCelular, IRepositorioIncidenciasAgua iiAgua, IRepositorioIncidenciasResiduos iiResiduos,
            IRepositorioIncidenciasConvencional iiConvencional, IRepositorioIncidenciasAnalisis iiAnalisis, IRepositorioIncidenciasTransporte iiTransporte,
            IRepositorioIncidenciasComedor iiComedor, IRepositorioCuestionarios iQuestion, IRepositorioVariables iVariables, IRepositorioFirmantes iFirmantes)
        {
            this.web = vweb;
            this.vCedula = viCedula;
            this.vQuestion = iQuestion;
            this.vrCedula = vvReporte;
            this.vVariables= iVariables;
            this.iLimpieza = iiLimpieza;
            this.iComedor = iiComedor;
            this.iFumigacion = iiFumigacion;
            this.iMensajeria = iiMensajeria;
            this.iCelular = iiCelular;
            this.iMuebles = IiMuebles;
            this.iConvencional = iiConvencional;
            this.iAgua = iiAgua;
            this.iResiduos = iiResiduos;
            this.iAnalisis = iiAnalisis;
            this.iTransporte = iiTransporte;
            this.iTraslado = iiTraslado;
            this.vFirmantes = iFirmantes;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/cedula/mensajeria/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaMensajeria(int servicio, int id)
        {
            var incidencias = new List<IncidenciasMensajeria>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaMensajeria.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                incidencias = await iMensajeria.getIncidenciasByTipoMensajeria(id, tiposIncidencia[i]);
                tiposIncidencia[i] = tiposIncidencia[i] == "Mal Estado" ? "MalEstado" : tiposIncidencia[i];
                local.DataSources.Add(new ReportDataSource("Incidencias" + tiposIncidencia[i], incidencias));
                if (i < 3)
                {
                    if (respuestas[i].Respuesta == false && !respuestas[i].Detalles.Equals("N/A"))
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else if (respuestas[i].Respuesta == false && respuestas[i].Detalles.Equals("N/A"))
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica la entrega de acuses en el mes de evaluación.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else
                {
                    if (i == 6)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó suficiente material de embalaje para las guías.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio entregó suficiente material de embalaje para las guías.") });
                        }
                    }
                    else
                    {
                        if (respuestas[i].Respuesta == true)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        }
                    }
                }                
            }
            local.DataSources.Add(new ReportDataSource("CedulaMensajeria", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/limpieza/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaLimpieza(int servicio, int id)
        {
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaLimpieza.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            List<IncidenciasLimpieza> incidencias = new List<IncidenciasLimpieza>();
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (i == 1)
                {
                    incidencias = await iLimpieza.getIncidenciasByPregunta(id,2);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPO", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de limpieza no portó identificación y/o uniforme en todo momento.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de limpieza portó identificación y/o uniforme en todo momento.") });
                    }
                }
                else if (i == 3)
                {
                    incidencias = await iLimpieza.getIncidenciasByPregunta(id, 4);
                    local.DataSources.Add(new ReportDataSource("IncidenciasEquipo", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no presentó inasistencias en el mes de evaluación.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio presentó " + respuestas[i].Detalles + " inasistencias del personal de limpieza en el mes de evaluación.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el SUA de su personal en tiempo y forma, " +
                            "la administración comentó lo siguiente: "+respuestas[i].Detalles)});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio entregó el SUA de su personal en tiempo y forma, la fecha de entrega" +
                                " fue el " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("yyyy") + " correspondiente al mes de " +
                                       respuestas[i].Detalles.Split("|")[1] + ".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaLimpieza", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/fumigacion/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaFumigacion(int servicio, int id)
        {
            var incidencias = new List<IncidenciasFumigacion>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaFumigacion.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                incidencias = new List<IncidenciasFumigacion>();
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasFechas", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasFechas", incidencias));
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasHora", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasHora", incidencias));
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasFauna", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasFauna", incidencias));
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la regulación vigente de los productos.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la regulación vigente de los productos.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se entregó el reporte de servicios por parte del prestador del servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El reporte de servicios se entregó " +
                            "el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                   Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                }
                else if (i == 6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se entregó el listado del personal por parte del prestador del servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El listado del personal se entregó " +
                            "el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                   Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                }
                else if (i == 7)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el SUA de su personal, comentó lo siguiente: "
                            +respuestas[i].Detalles) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio entregó el SUA de su personal, la fecha de entrega" +
                                " fue el " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("yyyy") + " correspondiente al mes de " +
                                       respuestas[i].Detalles.Split("|")[1] + ".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaFumigacion", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/muebles/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaMuebles(int servicio, int id)
        {
            var incidencias = new List<IncidenciasMuebles>();
            LocalReport local = new LocalReport();
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var cedula = await vCedula.CedulaById(id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            var path = "";
            if ((cedula.Mes.Equals("Enero") || cedula.Mes.Equals("Febrero") || cedula.Mes.Equals("Marzo") || cedula.Mes.Equals("Abril") || cedula.Mes.Equals("Mayo"))
                && cedula.Anio == 2022)
            {
                path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaBienesMuebles.rdlc";
                local.ReportPath = path;
                for (int i = 0; i < respuestas.Count; i++)
                {
                    if (i == 0)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la fecha y hora solicitada para la prestación del servicio, la fecha en que se solicitó fue: " +
                                     Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]) +" y la fecha y hora de llegada fue:"+ Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1]))});
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la fecha y hora solicitada para la prestación del servicio.") });
                        }
                    }
                    else if (i == 1)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasPregunta2", incidencias));
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });

                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });
                        }
                    }
                    else if (i == 2)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasPregunta3", incidencias));
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                        }
                    }
                    else if (i == 3)
                    {
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con el personal necesario para realizar la prestación del servicio, se solicitaron "+
                            respuestas[i].Detalles.Split("|")[0]+" persona(s) y el personal proporcionado fue de "+respuestas[i].Detalles.Split("|")[1]+" persona(s) para efectuar el servicio.") });
                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con el personal necesario para realizar la prestación del servicio.") });
                        }
                    }
                    else if (i == 4)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasMuebles", incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación:") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con las actividades contempladas en el programa de operación.") });
                        }
                    }
                }
            }
            else
            {
                path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaBienesMueblesV2.rdlc";
                local.ReportPath = path;
                for (int i = 0; i < respuestas.Count; i++)
                {
                    if (respuestas[i].Pregunta == 1)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                    else if (respuestas[i].Pregunta == 2)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasMuebles", incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación:") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con las actividades contempladas en el programa de operación.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 3)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la fecha y hora solicitada para la prestación del servicio, la fecha en que se solicitó fue: " +
                                     Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]) +" y la fecha y hora de llegada fue:"+ Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1]))});
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la fecha y hora solicitada para la prestación del servicio.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 4)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasPregunta4", incidencias));
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });

                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 5)
                    {
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 6)
                    {
                        if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con el personal necesario para realizar la prestación del servicio, se solicitaron "+
                            respuestas[i].Detalles.Split("|")[0]+" persona(s) y el personal proporcionado fue de "+respuestas[i].Detalles.Split("|")[1]+" persona(s) para efectuar el servicio.") });
                        }
                        else if (respuestas[i].Respuesta == false && respuestas[i].Detalles == "N/A")
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con el personal necesario para realizar la prestación del servicio.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 7)
                    {
                        incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                        local.DataSources.Add(new ReportDataSource("IncidenciasPregunta7", incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con los tiempos de entrega, las cuales se describen a continuación:") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con los tiempos de entrega.") });
                        }
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaMuebles", cedulas));
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/trasladoExp/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaTraslado(int servicio, int id)
        {
            var incidencias = new List<IncidenciasTraslado>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaTraslado.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    incidencias = await iTraslado.getIncidenciasByPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta1", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no presentó el personal solicitado para prestar el servicio, a continuación se indican las incidencias presentadas: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio presentó el personal solicitado para prestar el servicio.") });
                    }
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        if (!respuestas[i].Detalles.Equals("") && respuestas[i].Detalles != null)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se presentó debidamente uniformado e identificado al prestar el servicio, los comentarios capturados son los siguientes: " + respuestas[i].Detalles) });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se presentó debidamente uniformado e identificado al prestar el servicio.") });
                        }
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se presentó debidamente uniformado e identificado al prestar el servicio.") });
                    }
                }
                else if (i == 2)
                {
                    incidencias = await iTraslado.getIncidenciasByPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta3", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la maquinaria, equipo y herramientas para la prestación del servicio, a continuación se describen las incidencias presentadas: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaTraslado", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/celular/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaCelular(int servicio, int id)
        {
            var incidencias = new List<IncidenciasCelular>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaCelular.rdlc";
            local.ReportPath = path;
            int i = 0;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (i = 0; i < respuestas.Count; i++)
            {
                if (i != 0)
                {
                    incidencias = await iCelular.ListIncidenciasTipoCelular(id, tiposCelular[i]);
                    local.DataSources.Add(new ReportDataSource("Incidencias" + tiposCelular[i], incidencias));
                }
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (respuestas[i].Respuesta == true)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron " + rCelular[i] + " en el mes y se describen a continuación: ") });
                }
                else
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron " + rCelular[i] + " en el mes.") });
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaCelular", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/convencional/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaConvencional(int servicio, int id)
        {
            var incidencias = new List<IncidenciasConvencional>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaConvencional.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                incidencias = await iConvencional.ListIncidenciasTipoConvencional(id, tiposConv[i]);
                local.DataSources.Add(new ReportDataSource("Incidencias" + tiposConv[i], incidencias));
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (respuestas[i].Respuesta == true)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron solicitudes de " + rConv[i] + " en el mes y se describen a continuación: ") });
                }
                else
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron solicitudes de " + rConv[i] + " en el mes.") });
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaConvencional", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/agua/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaAgua(int servicio, int id)
        {
            var incidencias = new List<IncidenciasAgua>();
            LocalReport local = new LocalReport();
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var cedula = await vCedula.CedulaById(id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            if (respuestas.Count == 6)
            {
                var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaAguaParaBeberV1.rdlc";
                local.ReportPath = path;
                for (int i = 0; i < respuestas.Count; i++)
                {
                    if (i == 0)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                    else if (respuestas[i].Pregunta == 2 || respuestas[i].Pregunta == 3 || respuestas[i].Pregunta == 4)
                    {
                        incidencias = await iAgua.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("Incidencias" + incidenciasAgua[i], incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta, "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        }
                    }
                    else if (i == 4)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la entrega de los resultados de análisis microbiológicos.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la entrega de los resultados de análisis microbiológicos.") });
                        }
                    }
                    else if (i == 5)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con las normas oficiales NOM-201-SSA-2015.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con las normas oficiales NOM-201-SSA-2015.") });
                        }
                    }
                }
            }
            else
            {
                var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaAguaParaBeber.rdlc";
                local.ReportPath = path;
                for (int i = 0; i < respuestas.Count; i++)
                {
                    if (i == 0)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                    else if (respuestas[i].Pregunta == 2 || respuestas[i].Pregunta == 3)
                    {
                        incidencias = await iAgua.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("Incidencias" + respuestas[i].Pregunta, incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta, "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                        }
                    }
                    else if (respuestas[i].Pregunta == 4)
                    {
                        incidencias = await iAgua.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("Incidencias" + respuestas[i].Pregunta, incidencias));
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con la entrega de los resultados de análisis microbiológicos.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con la entrega de los resultados de análisis microbiológicos.") });
                        }
                    }
                    else if (i == 4)
                    {
                        if (respuestas[i].Respuesta == false)
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no cumplió con las normas oficiales NOM-201-SSA-2015.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio cumplió con las normas oficiales NOM-201-SSA-2015.") });
                        }
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaAgua", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/residuos/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaRPBI(int servicio, int id)
        {
            var incidencias = new List<IncidenciasResiduos>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaResiduos.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {

                if (i == 0)
                {
                    if (respuestas[i].Respuesta == false && !respuestas[i].Detalles.Split("|")[0].Equals("Anticipada"))
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se realizó en el día programado, se programó en la fecha "+Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("yyyy")+" y se llevó a cabo en la fecha " +
                                   Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1]).ToString("dd") + " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " "
                                   + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1]).ToString("yyyy") + ".")});
                    }
                    else if (respuestas[i].Respuesta == false && respuestas[i].Detalles.Split("|").Length == 3)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio se realizó de manera anticipada sin que ello implique una afectación al servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio se llevó a cabo en el día programado.") });
                    }
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal del prestador del servicio no portó identificación en todo momento.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal del prestador del servicio portó identificación en todo momento.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La recolección no se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La recolección se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.") });
                    }
                }
                else if (i == 3)
                {
                    incidencias = await iResiduos.getIncidenciasTipo(id, "ManifiestoEntrega");
                    local.DataSources.Add(new ReportDataSource("IncidenciasManifiesto", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Faltaron datos en el manifiesto de entrega, los cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El manifiesto de entrega se entregó con todos los datos necesarios.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iResiduos.getIncidencias(id);
                    local.DataSources.Add(new ReportDataSource("IncidenciasResiduos", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal del prestador del servicio no utilizó equipo de protección en todo momento, a continuación se describen las incidencias presentadas:") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal del prestador del servicio utilizó equipo de protección en todo momento.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaResiduos", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/analisis/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaAnalisis(int servicio, int id)
        {
            var incidencias = new List<IncidenciasAnalisis>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaAnalisis.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se realizó el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +" del mes "+ Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))+
                        " de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        //local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se llevo a cabo en la fecha programada, la fecha en que se realizo "+
                        //    " fue el " + Convert.ToDateTime(respuestas[i].Detalles.Split("")[1]).ToString("dd") +
                        //               " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("")[1]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                        //               Convert.ToDateTime(respuestas[i].Detalles.Split("")[1]).ToString("yyyy") +".") });
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se llevó a cabo en la fecha programada.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio se llevó a cabo en la fecha programada.") });
                    }
                }
                else if (i == 2)
                {
                    incidencias = await iAnalisis.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasAnalisis", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio no cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio, se describen las incidencias: .") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.") });
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no portó uniforme e identificación en todo momento: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio portó uniforme e identificación en todo momento.") });
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio recolectó un total de " + respuestas[i].Detalles + " muestras, no cumpliendo " +
                        " con el número de muestras mínimas de 7.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio recolectó el número total de muestras solicitadas.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no realizó el servicio en la fecha programada.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio realizó el servicio en la fecha programada.") });
                    }
                }
                else if (i == 6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el reporte del servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio entregó el reporte del servicio el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd")+" de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))
                        +" de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaAnalisis", cedulas));
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/transporte/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaTransporte(int servicio, int id)
        {
            var incidencias = new List<IncidenciasTransporte>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaTransporte.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se realizó el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +" del mes "+ Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))+
                        " de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                }
                else if (i == 1)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio presentó incidencias en el mes, se describen a continuación:") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio cumplió con los horarios y recorridos establecidos en el contrato.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se llevó a cabo el registro de los servidores públicos en este mes, los comentarios son los siguientes: " +
                        respuestas[i].Detalles) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se llevó a cabo el registro de los servidores públicos en este mes.") });
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), 
                            respuestas[i].Detalles) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La ocupación promedio fue mayor o igual al 50%.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con la supervisión del servicio, se presentaron las siguientes incidencias: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con la supervisión del servicio.") });
                    }
                }
                else if (i == 5)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se comportó de forma cortés, amable y portó el uniforme e identificación de forma correcta. ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se comportó de forma cortés, amable y portó el uniforme e identificación de forma correcta.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaTransporte", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/transporteNC/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaTransporteNuevoContrato(int servicio, int id)
        {
            var incidencias = new List<IncidenciasTransporte>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaTransporteNC.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se realizó el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +" de "+ Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))+
                        " de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                }
                else if (i == 1)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios no cumplió con el servicio de transporte conforme a los plazos y condiciones establecidas: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios cumplió con el servicio de transporte conforme a los plazos y condiciones establecidas.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios no proporcionó los entregables en los plazos establecidos: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios proporcionó los entregables en los plazos establecidos.") });
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se proporcionó con la infraestructura técnica, mecánica, tecnológica y humana establecidas en el anexo técnico: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio se proporcionó con la infraestructura técnica, mecánica, tecnológica y humana establecidas en el anexo técnico.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal operador del servicio no está capacitado ni cuenta con la experiencia conforme a lo establecido en el anexo técnico: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal operador del servicio está capacitado y cuenta con la experiencia conforme a lo establecido en el anexo técnico.") });
                    }
                }
                else if (i == 5)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal operador del servicio no cumplió con las obligaciones generales establecidas en el anexo técnico: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal operador del servicio cumplió con las obligaciones generales establecidas en el anexo técnico.") });
                    }
                }
                else if (i == 6)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios no cumplió con los horarios y rutas establecidos en el programa de operación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios cumplió con los horarios y rutas establecidos en el programa de operación.") });
                    }
                }               
            }
            local.DataSources.Add(new ReportDataSource("CedulaTransporte", cedulas));
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/comedor/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaComedor(int servicio, int id)
        {
            List<IncidenciasComedor> incidencias = null;
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaComedor Terminada.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var preguntas = await vQuestion.GetCuestionarioCompleto(servicio);
            var preguntasServicio = await vQuestion.GetCuestionarioByServicio(servicio,id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            var firmantes = await vFirmantes.GetFirmantesByCedula(id, servicio);
            local.DataSources.Add(new ReportDataSource("Firmantes", firmantes));
            for (int i = 0, j=0; i<preguntas.Count;i++)
            {
                incidencias = new List<IncidenciasComedor>();
                var exists = existsQuestion(preguntas[i].NoPregunta,preguntasServicio);
                if (exists)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta"+preguntas[i].NoPregunta, (j + 1) + ".- " +
                    (preguntas[i].NoPregunta == 22 && ((List<ReporteCedula>)cedulas)[0].TipoServicio.Equals("Comedor") ? preguntas[i].Pregunta:
                    preguntas[i].Pregunta.Replace("comedor general",CultureInfo.CurrentCulture.TextInfo.ToLower(((List<ReporteCedula>)cedulas)[0].TipoServicio)))) });

                    if (respuestas[j].Respuesta)
                    {
                        if (preguntas[i].NoPregunta == 10 || preguntas[i].NoPregunta == 11)
                        {
                            local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, (preguntas[i].Respuesta.Split("|")[1]).Replace("@rubro", "\""+preguntasServicio[j].Concepto+"\"")) });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, (preguntas[i].Respuesta.Split("|")[0]).Replace("@rubro", "\""+preguntasServicio[j].Concepto+"\"")) });
                        }
                    }
                    else //si la respuesta es falso
                    {
                        if (respuestas[j].Detalles != null)
                        {
                            if (preguntas[i].Incidencias && respuestas[j].Detalles.Equals("N/A"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No aplica para el periodo de evaluación.") });
                            }
                            else if (preguntas[i].Incidencias && respuestas[j].Detalles.Equals("N/R"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No se realizó en el periodo de evaluación.") });
                            }
                            else if (preguntas[i].Incidencias && respuestas[j].Detalles.Equals("N/E"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No se entregó en el periodo de evaluación.") });
                            }
                            else if (preguntas[i].Incidencias)
                            {
                                if (preguntas[i].NoPregunta == 10 || preguntas[i].NoPregunta == 11)
                                {
                                    local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, preguntas[i].Respuesta.Split("|")[0]) });
                                }
                                else
                                {
                                    local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, preguntas[i].Respuesta.Split("|")[1]) });
                                }
                            }
                            else if(respuestas[j].Detalles.Equals("N/A"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No aplica para el periodo de evaluación.") });
                            }
                            else if (respuestas[j].Detalles.Equals("N/R"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No se realizó en el periodo de evaluación.") });
                            }
                            else if (respuestas[j].Detalles.Equals("N/E"))
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "No se entregó en el periodo de evaluación.") });
                            }
                            else if (preguntas[i].Cierre)
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, "El cierre del servicio se efectuó el "+
                                Convert.ToDateTime(respuestas[j].Detalles).ToString("dd/MM/yyy")) });
                            }
                            else if (preguntas[i].Fechas)
                            {
                                local.SetParameters(new[] { new ReportParameter("respuesta" + preguntas[i].NoPregunta, preguntas[i].Respuesta.Split("|")[1]+" "+
                                    "la fecha programada era el "+Convert.ToDateTime(respuestas[j].Detalles.Split("|")[0]).ToString("dd/MM/yyyy")+" y se realizó el "+
                                    Convert.ToDateTime(respuestas[j].Detalles.Split("|")[1]).ToString("dd/MM/yyyy")+"\n") });
                            }
                        }
                    }


                    if (preguntas[i].Incidencias)
                    {
                        incidencias = await iComedor.GetIncidenciasPregunta(id, preguntas[i].NoPregunta);
                        local.DataSources.Add(new ReportDataSource("ICP" + preguntas[i].NoPregunta, incidencias));

                    }
                    j++;
                }
                else
                {
                    if (preguntas[i].Incidencias)
                    {
                        local.DataSources.Add(new ReportDataSource("ICP" + preguntas[i].NoPregunta, incidencias));
                    }
                }
            }

            local.DataSources.Add(new ReportDataSource("CedulaComedor", cedulas));
            
            local.SetParameters(new[] { new ReportParameter("elaboro", ((List<ReporteCedula>)cedulas)[0].Elaboro + "") });
            local.SetParameters(new[] { new ReportParameter("periodo", ((List<ReporteCedula>)cedulas)[0].Periodo + "") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        private bool existsQuestion(int pregunta, List<Preguntas> preguntasServicio)
        {
            for(var i = 0;i< preguntasServicio.Count;i++)
            {
                if (preguntasServicio[i].NoPregunta == pregunta)
                {
                    return true;
                }
            }
            return false;
        }

        [Route("/financieros/limpieza")]
        public async Task<IActionResult> ReporteLimpieza()
        {
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\ReporteFinancieros.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getReporteFinancierosLimpieza();
            local.DataSources.Add(new ReportDataSource("ReporteFinancierosLimpieza", cedulas));

            var excel = local.Render("EXCELOPENXML");
            return File(excel, "application/msexcel", "ReporteLimpieza_" + DateTime.Now + ".xls");
        }
    }
}
