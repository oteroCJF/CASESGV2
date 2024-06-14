using CedulasEvaluacion.Entities.MCedulaR;
using CedulasEvaluacion.Entities.MVariables;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class CedulaResultadosController : Controller
    {
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioContratosServicio vContrato;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioCedulaResultados vResultados;
        private readonly IRepositorioPerfiles vPerfiles;

        public CedulaResultadosController(IRepositorioPerfiles iPerfiles, IRepositorioCatalogoServicios iCatalogo, IRepositorioContratosServicio iContrato, 
                                            IRepositorioVariables iVariables, IRepositorioCedulaResultados iResultados)
        {
            this.vCatalogo = iCatalogo ?? throw new ArgumentNullException(nameof(iCatalogo));
            this.vContrato = iContrato ?? throw new ArgumentNullException(nameof(iContrato));
            this.vResultados = iResultados ?? throw new ArgumentNullException(nameof(iResultados));
            this.vVariables= iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        [Route("/cedResultados/index")]
        public async Task<IActionResult> Index([FromQuery(Name = "servicio")] int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                ModelosCR modelos = new ModelosCR();
                modelos.ServicioId = servicio;
                modelos.contratos = await vContrato.GetContratosServicios(servicio);
                modelos.servicios = await vCatalogo.GetCatalogoServicios();
                modelos.anios = await vVariables.GetAniosEvaluacion();
                modelos.meses = await vVariables.GetMesesEvaluacion();
                modelos.rubros = await vVariables.GetRubrosEvaluar(servicio);
                return View(modelos);
            }

            return Redirect("/error/denied");
        }

        [Route("/cedulaResultados/{servicio}/{anio}/{mesI}/{mesF}/{rubros}/{contrato}")]
        public async Task<IActionResult> GeneraCedulaResultados(int servicio, int anio, int mesI, int mesF, string rubros, int contrato)
        {
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaResultados.rdlc";
            local.ReportPath = path;
            IEnumerable<MIndiceEfectividad> indices = await vResultados.GetIndiceEfectividad(servicio,anio, mesI, mesF, rubros,contrato);
            IEnumerable<Variables> rEvaluar = await vResultados.GetRubrosaEvaluar(rubros);
            IEnumerable<PeriodoEvaluacion> periodo = await vResultados.GetPeriodoEvaluacion(anio, mesI, mesF, servicio,contrato);
            IEnumerable<MIndiceEfectividad> incidencias = await vResultados.GetIncidenciasInmueble(servicio, anio, mesI, mesF, rubros,contrato);
            IEnumerable<AvanceFinanciero> af = await vResultados.GetAvanceFinanciero(servicio, contrato);
            IEnumerable<AvanceFisico> afi= await vResultados.GetAvanceFisico(servicio, contrato);
            local.DataSources.Add(new ReportDataSource("PeriodoEvaluacion", periodo));
            local.DataSources.Add(new ReportDataSource("RubrosEvaluar", rEvaluar));
            local.DataSources.Add(new ReportDataSource("IndicesEfectividad", indices));
            local.DataSources.Add(new ReportDataSource("Incidencias", incidencias));
            local.DataSources.Add(new ReportDataSource("AvanceFinanciero", af));
            local.DataSources.Add(new ReportDataSource("AvanceFisico", afi));
            local.SetParameters(new[] { new ReportParameter("servicio", periodo.First().Servicio) });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Cédula_de_Resultados";
        }
    }
}
