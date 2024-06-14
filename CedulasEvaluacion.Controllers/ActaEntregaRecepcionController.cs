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
using CedulasEvaluacion.Entities.MIncidencias;

namespace CedulasEvaluacion.Controllers
{
    public class ActaEntregaRecepcionController : Controller
    {

        private string[] tiposConv = { "contratacion_instalacion", "cableado", "entregaAparato", "cambioDomicilio", "reubicacion", "identificadorLlamadas", "troncales", "internet",
                                          "serviciosTelefonia", "cancelacion", "reportesFallas"};

        private string[] rConv = { "nuevas contrataciones", "cableados interiores para la instalación de líneas telefónicas","entregas de aparatos telefónicos", "cambios de domicilio",
            "reubicaciones de lineas telefónicas comerciales en el mismo inmueble", "activaciones para el identificador de llamadas",
            "instalaciones de troncales y DID's",
                                   "solicitudes de instalación de internet", "habilitación de servicios",
                                          "solicitudes de cancelación de servicios", "reportes de fallas"};

        private readonly IWebHostEnvironment web;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioReporteCedula vrCedula;

        public ActaEntregaRecepcionController(IWebHostEnvironment vweb, IRepositorioReporteCedula vvReporte, IRepositorioIncidenciasMensajeria iiMensajeria,
           IRepositorioIncidencias iiLimpieza, IRepositorioEvaluacionServicios viCedula, IRepositorioIncidenciasFumigacion iiFumigacion,
           IRepositorioIncidenciasMuebles IiMuebles, IRepositorioIncidenciasCelular iiCelular,
           IRepositorioIncidenciasConvencional iiConvencional)
        {
            this.web = vweb;
            this.vCedula = viCedula;
            this.vrCedula = vvReporte;
            this.iConvencional = iiConvencional;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/acta/convencional/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaConvencional(int servicio, int id)
        {
            var incidencias = new List<IncidenciasConvencional>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\ActaERConvencional.rdlc";
            local.ReportPath = path;
            local.SetParameters(new[] { new ReportParameter("p1", "<p><b>Acta de entrega – recepción mensual</b> del \"Servicio de Telefonía Convencional y Servicios Adicionales\" adjudicado a la empresa TELÉFONOS DE MÉXICO, S.A.B. DE C.V., mediante el contrato CON/DGRM/DCS/051/2021, por el periodo comprendido del 1 de enero de 2021 al 31 de marzo de 2023, en lo relativo la Dirección de Administración de Servicios, con domicilio en Carretera Picacho Ajusco 170, Colonia Jardines en la Montaña, C.P. 14210, Alcaldía Tlalpan.") });
            var pdf = local.Render("WORDOPENXML");
            return File(pdf, "application/msword", "ActaER_" + DateTime.Now + ".docx");
        }
    }
}
