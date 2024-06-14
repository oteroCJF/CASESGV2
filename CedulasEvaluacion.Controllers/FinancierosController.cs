using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MFinancieros;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Ionic.Zip;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class FinancierosController : Controller
    {
        private readonly IRepositorioFinancieros vFinancieros;
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioPerfiles vPerfiles;

        public FinancierosController(IRepositorioFinancieros ivFinancieros, IRepositorioCatalogoServicios ivCatalogo,
            IRepositorioEntregablesCedula iVEntregables, IRepositorioPerfiles ivPerfiles, IRepositorioInmuebles iInmuebles,
            IRepositorioVariables iVariables)
        {
            this.vFinancieros = ivFinancieros ?? throw new ArgumentNullException(nameof(ivFinancieros));
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
            this.vCatalogo = ivCatalogo ?? throw new ArgumentNullException(nameof(ivCatalogo));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vInmuebles = iInmuebles ?? throw new ArgumentNullException(nameof(iInmuebles));
        }

        [HttpGet]
        [Route("/financieros/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<DashboardFinancieros> resultado = new List<DashboardFinancieros>();
                resultado = await vFinancieros.GetCedulasFinancieros();
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/detalle/{servicio}")]
        public async Task<IActionResult> DetalleServicio(string servicio, [FromQuery(Name = "Anio")] int Anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                ModelsFinancieros models = new ModelsFinancieros();
                models.Anio = Anio != 0 ? Anio : DateTime.Now.Year;
                models.Servicio = servicio;
                models.ServicioId = ((CatalogoServicios)await vCatalogo.GetDescripcionServicio(servicio)).Id;
                models.Descripcion = ((CatalogoServicios)await vCatalogo.GetDescripcionServicio(servicio)).Descripcion;
                models.inmuebles = await vInmuebles.getInmuebles();
                models.listaEntregables = await vVariables.GetListadoEntregables(((CatalogoServicios)await vCatalogo.GetDescripcionServicio(servicio)).Id);
                models.dashboard = new List<DashboardFinancieros>();
                models.dashboard = await vFinancieros.GetDetalleServicio(servicio, Anio);
                models.oficio = await vFinancieros.GetOficiosFinancieros(servicio, Anio);
                return View(models);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/financieros/descargaentregables")]
        public async Task<string> DescargarEntregables([FromBody] List<Entregables> entregables)
        {
            string archivoO = "";
            var i = 0;
            string fecha = DateTime.Now.ToString("yyyMMddHHmmss");
            //string archivoD = "D:\\CASESGV2\\Financieros";
            string archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Financieros";

            if (Directory.Exists(archivoD))
            {
                Directory.Delete(archivoD, true);
            }

            Directory.CreateDirectory(archivoD);

            List<Entregables> actas = await vEntregables.DescargarEntregables(entregables);
            try
            {
                foreach (var ac in actas)
                {
                    i++;
                    //archivoO = "D:\\Entregables\\CASESGV2\\" + ac.Folio + "\\" + ac.NombreArchivo;
                    archivoO = Directory.GetCurrentDirectory()+"\\Entregables\\" + ac.Folio + "\\" + ac.NombreArchivo;
                    archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Financieros\\" + i + "_" + ac.Tipo + "_" +ac.Folio +"_"+ac.ClienteEstafeta+ ".pdf";
                    //archivoD = "D:\\CASESGV2\\Financieros\\" + i + "_" + ac.Tipo + "_" + ac.Folio + "_" + ac.ClienteEstafeta + ".pdf";
                    var file = new FileInfo(archivoO);
                    var fileD = new FileInfo(archivoD);


                    if (System.IO.File.Exists(archivoO))
                    {
                        file.CopyTo(archivoD);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Financieros";

            string archivoZip = "Entregables_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(archivoD);
                zip.Comment = "Archivo comprimido el " + System.DateTime.Now.ToString("G");
                zip.Save(Directory.GetCurrentDirectory() + "\\Entregables\\Financieros\\" + archivoZip + ".zip");
            }

            return !archivoZip.Equals("") ? archivoZip:"";
        }

        [HttpGet]
        [Route("/financieros/descargaEntregables/{entregables}")]
        public async Task<IActionResult> DescargaEntregables(string entregables)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Entregables\\Financieros\\" + entregables + ".zip");
            string fileName = entregables + ".zip";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [Route("/financieros/oficio/{servicio}/{id}")]
        public async Task<IActionResult> NuevoOficio(int id, int servicio, [FromQuery(Name = "idc")] int idc, [FromQuery(Name = "mes")] string mes)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                Oficio oficio = await vFinancieros.GetOficioById(id);
                oficio.detalleCedulas = new List<DetalleCedula>();
                oficio.Mes = mes;
                oficio.InmuebleId = idc;
                if (idc != 0 || mes != null) 
                {
                    oficio.detalleCedulas = await vFinancieros.GetCedulasFiltroPago(idc, mes, servicio);
                    oficio.facturas = await vFinancieros.GetFacturasFiltroPago(idc, servicio,mes);
                }
                else
                {
                    oficio.detalleCedulas = await vFinancieros.GetCedulasTramitePago(id, servicio);
                    oficio.facturas = await vFinancieros.GetFacturasTramitePago(id, servicio);
                }
                oficio.cedulasOficio = new List<DetalleCedula>();
                oficio.cedulasOficio = await vFinancieros.GetFacturasOficio(id, servicio);
                return View(oficio);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/revisar/{servicio}/{id}")]
        public async Task<IActionResult> RevisarOficio(int id, int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                Oficio oficio = await vFinancieros.GetOficioById(id);
                oficio.detalleCedulas = new List<DetalleCedula>();
                oficio.detalleCedulas = await vFinancieros.GetCedulasTramitePago(id, servicio);
                oficio.cedulasOficio = new List<DetalleCedula>();
                oficio.cedulasOficio = await vFinancieros.GetFacturasOficio(id, servicio);
                return View(oficio);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/envia/oficio/{id}/{servicio}")]
        public async Task<IActionResult> TramitarOficio(int id, int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                int tramitado = await vFinancieros.TramitarOficioDGPPT(id, servicio, UserId());
                if (tramitado != -1)
                {
                    return Ok(tramitado);
                }
                return BadRequest();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/cancela/oficio/{servicio}/{id}")]
        public async Task<IActionResult> CancelarOficio(int id, int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                int cancelado = await vFinancieros.CancelarOficio(id, servicio, UserId());
                if (cancelado != -1)
                {
                    return Ok(cancelado);
                }
                return BadRequest();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/pagar/oficio/{servicio}/{id}/{fecha}")]
        public async Task<IActionResult> PagarOficio(int servicio, int id,DateTime fecha)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                int cancelado = await vFinancieros.PagarOficio(id, servicio,fecha,UserId());
                if (cancelado != -1)
                {
                    return Ok(cancelado);
                }
                return BadRequest();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/elimina/cedulasOficio/{oficio}/{servicio}/{factura}")]
        public async Task<IActionResult> EliminaCedulasOficio(int oficio, int servicio, int factura)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                int elimina = await vFinancieros.EliminaCedulasOficio(oficio, servicio, factura);
                if (elimina != -1)
                {
                    return Ok(elimina);
                }
                return BadRequest();
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/financieros/inserta/oficio")]
        public async Task<IActionResult> InsertarOficio([FromBody] Oficio oficio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                int insert = await vFinancieros.insertarNuevoOficio(oficio);
                return Ok(insert);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/financieros/inserta/cedulasOficio")]
        public async Task<IActionResult> InsertarCedulasOficio([FromBody] List<CedulasOficio> cedulas)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                int insert = await vFinancieros.insertarCedulasOficio(cedulas);
                return Ok(insert);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/financieros/generaTabla/{servicio}/{flujo}/{anio}")]
        public async Task<IActionResult> generaFilas(string servicio, string flujo, int anio)
        {
            List<DashboardFinancieros> resultado = await vFinancieros.GetDetalleServicio(servicio, anio);

            var meses = obtieneMeses(resultado);
            var totales = obtieneTotales(resultado, meses);
            var estatus = obtieneEstatus(resultado, flujo);
            var columnas = new List<string>();
            var filas = new List<List<string>>();
            double total = 0;
            var text = "text-black";

            for (var i = 0; i < meses.Count; i++)
            {
                columnas = generaColumnas(estatus);
                for (var j = 0; j < resultado.Count; j++)
                {
                    text = "text-black";
                    if (meses[i] == resultado[j].Mes)
                    {
                        total = (resultado[j].Total * 100.0) / totales[i];
                        if (obtienePosicion(estatus, resultado[j].Estatus) != -1)
                        {
                            if (((resultado[j].Total * 100) / totales[i]) >= 63.0 && !resultado[j].Estatus.Equals("En Proceso"))
                            {
                                text = "text-light";
                            }

                            columnas[obtienePosicion(estatus, resultado[j].Estatus)] =
                               "<td>" +
                                   "<div class='row col-lg-12'>" +
                                       "<div class='col-lg-12 text-center'>" +
                                       "<span class='mb-2 badge " + resultado[j].Fondo + "'>" + resultado[j].Total + " Cédula(s)</span>" +
                                           "<div class='progress'>" +
                                               "<div class='progress-bar progress-bar-success progress-bar-striped progress-bar-animated " + resultado[j].Fondo + "' role='progressbar' " +
                                               "style='width: " + ((resultado[j].Total * 100) / totales[i]) + "%' aria-valuenow='"+ ((resultado[j].Total * 100) / totales[i]) + 
                                               "' aria-valuemin='0' aria-valuemax='100'>"+
                                                    "<span class='"+text+" font-weight-bold'> Avance de " + total.ToString("n2") + " % </span>" +
                                               "</div>" +
                                           "</div>" +
                                       "</div>" +
                                       (resultado[j].TotalParcial != 0 ? "<div class='col-lg-12'>" +
                                           "<div class='progress mt-2'>" +
                                               "<div class='progress-bar progress-bar-success progress-bar-striped progress-bar-animated bg-cedulasParciales' role='progressbar' " +
                                               "style='width: 100%'; >" +
                                                    "<span class='text-light font-weight-bold'>" + resultado[j].TotalParcial + " - cédula(s) parcialmente pagada(s)</span>" +
                                               "</div>" +
                                           "</div>" +
                                       "</div>":"")+
                                       (resultado[j].APendientes != 0 ? "<div class='col-lg-12'>" +
                                           "<div class='progress mt-2'>" +
                                               "<div class='progress-bar progress-bar-success progress-bar-striped progress-bar-animated bg-actasER' role='progressbar' " +
                                               "style='width: " + ((double)resultado[j].APendientes / (double)totales[i]) * 100.0 + "%'; >" +
                                                    "<span class='" + text + " font-weight-bold'>" + resultado[j].APendientes + " - Acta(s) Pendiente(s) de Firma </span>" +
                                               "</div>" +
                                           "</div>" +
                                       "</div>" : "") +
                                        (resultado[j].CedulasPendientes != 0 ? "<div class='col-lg-12'>" +
                                           "<div class='progress mt-2'>" +
                                               "<div class='progress-bar progress-bar-success progress-bar-striped progress-bar-animated bg-cedula' role='progressbar' " +
                                               "style='width: " + ((double)resultado[j].CedulasPendientes / (double)totales[i]) * 100.0 + "%'; >" +
                                                    "<span class='" + text + " font-weight-bold'>" +
                                                        resultado[j].CedulasPendientes + " - \"Cédula(s)\" pendiente(s) de validar " +
                                                    "</span>" +
                                               "</div>" +
                                           "</div>" +
                                       "</div>" : "") +
                                       (resultado[j].MemosPendientes != 0 ? "<div class='col-lg-12'>" +
                                           "<div class='progress mt-2'>" +
                                               "<div class='progress-bar progress-bar-success progress-bar-striped progress-bar-animated bg-memo' role='progressbar' " +
                                               "style='width: " + ((double)resultado[j].MemosPendientes / (double)totales[i]) * 100.0 + "%'; >" +
                                                    "<span class='" + text + " font-weight-bold'>" + 
                                                        resultado[j].MemosPendientes + " - \"Memorándum(s)\" pendiente(s) de validar "+
                                                    "</span>" +
                                               "</div>" +
                                           "</div>" +
                                       "</div>" : "") +
                                   "</div>" +
                               "</td>";
                        }
                    }
                }
                filas.Add(columnas);
            }

            return Ok(filas);
        }

        //Actualización de Oficio para carga del Acuse
        [HttpPost]
        [Route("/financieros/insertaAcuse")]
        public async Task<IActionResult> insertaAcuseOficio([FromForm] Oficio oficio)
        {
            int success = await vFinancieros.insertaAcuseOficio(oficio);
            if (success == 1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        public int obtienePosicion(List<string> estatus, string nEstatus)
        {
            int p = -1;
            for (var i = 0; i < estatus.Count; i++)
            {
                if (estatus.ElementAt(i) == nEstatus)
                {
                    p = i;
                    return p;
                }
            }
            return p;
        }

        public List<string> obtieneMeses(List<DashboardFinancieros> dashboards)
        {
            var meses = new List<string>();
            foreach (var dt in dashboards)
            {
                meses.Add(dt.Mes);
            }
            HashSet<string> quitaMeses = new HashSet<string>(meses);
            List<string> lmeses = quitaMeses.ToList();

            return lmeses;
        }

        public List<int> obtieneTotales(List<DashboardFinancieros> dashboards, List<string> meses)
        {
            var totales = new List<int>();
            for (var f = 0; f < meses.Count; f++)
            {
                var total = 0;
                for (var c = 0; c < dashboards.Count; c++)
                {
                    if (dashboards[c].Mes == meses[f])
                    {
                        total += dashboards[c].Total;
                    }
                }
                totales.Add(total);
            }

            return totales;
        }

        public List<string> generaColumnas(List<string> estatus)
        {
            var columnas = new List<string>();
            foreach (var dt in estatus)
            {
                columnas.Add("<td></td>");
            }

            return columnas;
        }

        public List<string> obtieneEstatus(List<DashboardFinancieros> dashboards, string flujo)
        {
            var estatus = new List<string>();
            if (flujo == "Operacion")
            {
                foreach (var dt in dashboards)
                {
                    if (dt.Estatus.Equals("En Proceso") || dt.Estatus.Equals("Enviado a DAS") || dt.Estatus.Equals("Autorizada") || dt.Estatus.Equals("Rechazada")
                        || dt.Estatus.Equals("Trámite Rechazado"))
                    {
                        estatus.Add(dt.Estatus);
                    }
                }
            }
            else if (flujo == "CAE")
            {
                foreach (var dt in dashboards)
                {
                    if (dt.Estatus.Equals("Revisión CAE") || dt.Estatus.Equals("Autorizado CAE"))
                    {
                        estatus.Add(dt.Estatus);
                    }
                }
            }
            else if (flujo == "Financieros")
            {
                foreach (var dt in dashboards)
                {
                    if (dt.Estatus.Equals("En Trámite") || dt.Estatus.Equals("Trámite de Pago") || dt.Estatus.Equals("Enviada a DGPPT") || dt.Estatus.Equals("Pagada"))
                    {
                        estatus.Add(dt.Estatus);
                    }
                }
            }
            HashSet<string> quitaEstatus = new HashSet<string>(estatus);
            List<string> lestatus = quitaEstatus.ToList();

            return lestatus;
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Financieros";
        }
    }
}
