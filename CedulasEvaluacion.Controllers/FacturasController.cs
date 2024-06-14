using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class FacturasController : Controller
    {
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public FacturasController(IRepositorioEvaluacionServicios viCedula, IRepositorioFacturas iFacturas, IRepositorioInmuebles viInmueble,
            IRepositorioPerfiles ivPerfiles, IHostingEnvironment environment)
        {
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vInmuebles = viInmueble ?? throw new ArgumentNullException(nameof(viInmueble));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
            this.environment = environment;
        }

        /************************* Modulo Facturas *****************************/
        [HttpGet]
        [Route("/facturas/index")]
        public async Task<IActionResult> Index([FromQuery(Name = "Anio")] int anio, [FromQuery(Name = "Servicio")] int Servicio, [FromQuery(Name = "Mes")] string Mes, 
            [FromQuery(Name = "Tipo")] string Tipo)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                ModelsFacturas resultado = new ModelsFacturas();
                resultado.Anio = anio;
                resultado.facturasServicio = await vFacturas.getFacturasTipo("Servicio",anio);
                resultado.facturasMes = await vFacturas.getFacturasTipo("Mes",anio);
                resultado.facturasParciales = await vFacturas.getFacturasTipo("Parciales",anio);
                if (Servicio != 0)
                {
                    resultado.detalle = await vFacturas.getResumenFacturacion(Servicio, anio);
                    resultado.desgloceServicio = await vFacturas.getDesgloceFacturacion(Servicio,anio);
                }
                return View(resultado);
            }
            return Redirect("/error/denied");
        }
        /*********************** Fin Modulo Facturas ***************************/

        /************************* Facturas *****************************/

        //obtenemos todas las facturas en base a la cedula
        [HttpGet]
        [Route("/limpieza/getFacturas/{id?}/{servicio?}")]
        public async Task<IActionResult> obtieneFacturas(int id,int servicio)
        {
            List<Facturas> facturas = await vFacturas.getFacturas(id,servicio);
            decimal total = 0;
            if (facturas != null)
            {
                return Ok(facturas);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/facturas/getFacturasSB/{id?}/{servicio?}")]
        public async Task<IActionResult> obtieneFacturasSB(int id,int servicio)
        {
            List<Facturas> facturas = await vFacturas.getFacturasSB(id,servicio);
            decimal total = 0;
            if (facturas != null)
            {
                return Ok(facturas);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/facturas/insertaFactura")]
        public async Task<IActionResult> insertaFacturas([FromForm] Facturas facturas)
        {
            CedulaEvaluacion cedula = await vCedula.CedulaById(facturas.CedulaId);
            if (cedula.Estatus.Equals("En Proceso") || cedula.Estatus.Equals("Rechazada") || cedula.Estatus.Equals("Trámite Rechazado") || cedula.Estatus.Equals("Autorizada")) {
                int success = await vFacturas.insertaConceptoFacturas(facturas);
                if (success == 1)
                {
                    return Ok(success);
                }
                else if (success != -1)
                {
                    cedula = await vCedula.CedulaById(success);
                    cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                    return Ok(cedula);
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/facturas/insertaFacturaSB")]
        public async Task<IActionResult> insertaFacturasSB([FromForm] Facturas facturas)
        {
            int success = await vFacturas.insertaConceptoFacturasSB(facturas);
            if (success == 1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/facturas/updateFactura")]
        public async Task<IActionResult> updateFacturas([FromForm] Facturas facturas)
        {
            CedulaEvaluacion cedula = await vCedula.CedulaById(facturas.CedulaId);
            if (cedula.Estatus.Equals("En Proceso") || cedula.Estatus.Equals("Rechazada") ||
                cedula.Estatus.Equals("Trámite Rechazado") || cedula.Estatus.Equals("Autorizada"))
            {
                int success = 0;
                success = await vFacturas.updateConceptoFacturas(facturas);
                if (success == 1)
                {
                    return Ok(success);
                }
                else if (success != -1)
                {
                    cedula = await vCedula.CedulaById(success);
                    cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                    return Ok(cedula);
                }
                return NoContent();
            }
            return NoContent();
        }

        [HttpPost]
        [Route("/facturas/updateFacturaSB")]
        public async Task<IActionResult> updateFacturasSB([FromForm] Facturas facturas)
        {
            int success = 0;
            success = await vFacturas.updateConceptoFacturasSB(facturas);
            if (success == 1)
            {
                return Ok(success);
            }

            return NoContent();
        }

        /*Metodo para eliminar Factura*/
        [HttpDelete]
        [Route("/facturas/deleteFactura/{factura?}")]
        public async Task<IActionResult> deleteFactura(int factura)
        {
            int success = 0;
            success = await vFacturas.deleteFactura(factura);
            if (success != 0)
            {
                return Ok(success);
            }
            return NoContent();
        }

        /************************* Fin Facturas Limpieza *****************************/

        /*Filtros de Facturas*/
        [HttpGet]
        [Route("/facturas/getFacturasPago/{servicio}")]
        public async Task<IActionResult> getInmueblesPago(int servicio)
        {
            List<Facturas> inmueble = null;
            inmueble = await vFacturas.getFacturasPago(servicio);
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        [Route("/facturas/download/{id}/{tipo}")]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(int id,string tipo)
        {
            string newPath = "";
            Facturas factura = await vFacturas.getFacturaById(id);
            if (tipo.Equals("NC"))
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Notas de Crédito";
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Facturas";
            }
            string fileName = factura.NombreArchivo;
            if (fileName != "")
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(newPath + "\\" + factura.NombreArchivo);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return Ok();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Facturación";
        }

        /*********************************** Inserta Masivo de Facturas **********************************************/
        [HttpGet]
        [Route("/limpieza/masivoFacturas")]
        public async Task<IActionResult> insertaMasivoFacturas()
        {   //"ACTUALIZACIÓN PRECIOS UNITARIOS (Enero) PERIODO:ENERO 2022. INMUEBLE: SEDE. COMPLEMENTO FACTURA A2357."
            var isNext = 0;
            int success = 0;
            string[] folios = { "202201", "202202", "202203", "202204" };
            List<string> xml = archivosXML();
            Facturas facturas = new Facturas();
            foreach (var x in xml)
            {
                //string file = Directory.GetCurrentDirectory() + "\\XLM NC (Pruebas)\\" + x;
                string file = Directory.GetCurrentDirectory() + "\\XLM NC (Pruebas)\\" + x;
                facturas.Xml = new FormFile(System.IO.File.OpenRead(file), 0, System.IO.File.OpenRead(file).Length, null, Path.GetFileName(System.IO.File.OpenRead(file).Name));
                success = await vFacturas.insertaMasivoConceptoFacturas(
                    facturas,
                    x.Split("_")[1],
                    x.Split("_")[0],
                    1
                    );
            }
            return BadRequest(isNext);
        }

        public List<string> archivosXML()
        {
            List<string> xml = new List<string>();
            DirectoryInfo carpeta = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\XLM NC (Pruebas)");
            foreach (var c in carpeta.GetFiles())
            {
                xml.Add(c.Name + "");
            }
            return xml;
        }
        /*********************************** Fin de Masivo de Facturas **********************************************/

    }
}
