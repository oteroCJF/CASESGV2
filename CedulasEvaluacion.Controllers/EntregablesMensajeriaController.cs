using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Ionic.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class EntregablesMensajeriaController : Controller
    {

        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IHostingEnvironment environment;

        public EntregablesMensajeriaController(IRepositorioEntregablesCedula iVEntregables, IRepositorioEvaluacionServicios iCedula, IHostingEnvironment environment)
        {
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
            this.vCedula = iCedula ?? throw new ArgumentNullException(nameof(iCedula));
            this.environment = environment;
        }

        /***************************** Mensajeria ****************************/
        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/mensajeria/adjuntaEntregable")]
        public async Task<IActionResult> adjuntaEntregable([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await vEntregables.adjuntaEntregable(entregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        /*Fin del Metodo para adjuntar los entregables*/

        /*Metodo para obtener las inasistencias*/
        [HttpGet]
        [Route("/mensajeria/getEntregables/{id?}/{servicio}")]
        public async Task<IActionResult> getEntregablesLimpieza(int id,int servicio)
        {
            List<Entregables> entregables = await vEntregables.getEntregables(id, servicio);
            CedulaEvaluacion cedula = await vCedula.CedulaById(id);
            string table = "";
            string tipo = "";
            if (entregables != null)
            {
                foreach (var entregable in entregables)
                {
                    table += "<tr>" +
                    "<td>" + entregable.TipoEntregable + "</td>" +
                    "<td>" + entregable.NombreArchivo + "</td>" +
                    "<td>" + entregable.FechaCreacion.ToString("yyyy-MM-dd") + "</td>" +
                    "<td>" +
                        "<a href='#' class='text-center mr-2 view_file' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo + "' data-tipo ='" + tipo + "'>" +
                        "<i class='fas fa-eye text-success'></i></a>" +
                        (cedula.Estatus.Equals("Rechazada") || cedula.Estatus.Equals("En Proceso") ?
                        "<a href='#' class='text-center mr-2 update_files' data-id='" + entregable.Id + "' data-coments='" + entregable.Comentarios + "' data-file='" + entregable.NombreArchivo + "'" +
                            "data-tipo='" + entregable.Tipo + "'><i class='fas fa-edit text-primary'></i></a>" +
                        "<a href='#' class='text-center mr-2 delete_files' data-id='" + entregable.Id + "' data-tipo='" + entregable.Tipo + "'><i class='fas fa-times text-danger'></i></a>"
                        : "")
                        +
                    "</td>" +
                    "</tr>";
                }
                return Ok(table);
            }
            return NoContent();
        }
        /*FIN del metodo para obtener las inasistencias*/

        /*Obtiene los tipos de entregables que ya fueron adjuntos*/
        [HttpGet]
        [Route("/mensajeria/getListadoEntregables/{id?}/{servicio}")]
        public async Task<IActionResult> getListadoEntregablesMensajeria(int id,int servicio)
        {
            List<Entregables> entregables = null;
            entregables = await vEntregables.getEntregables(id,servicio);
            if (entregables != null)
            {
                return Ok(entregables);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("/mensajeria/verArchivo/{folio?}/{nombre?}")]
        public IActionResult archivoProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpPost]
        [Route("/mensajeria/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] Entregables entregable)
        {
            int success = 0;
            success = await vEntregables.eliminaEntregables(entregable);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/mensajeria/buscaEntregable/{id?}/{tipo?}")]
        public async Task<IActionResult> buscaEntregable(int id, string tipo)
        {
            int exists = 0;
            exists = await vEntregables.buscaEntregable(id, tipo);
            if (exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/mensajeria/Entregables/autoRecha")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] Entregables entregables)
        {
            int success = await vEntregables.apruebaRechazaEntregable(entregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/mensajeria/entregables/historialEntregable")]
        public async Task<IActionResult> historialEntregable([FromBody] HistorialEntregables historialEntregables)
        {
            int success = 0;
            success = await vEntregables.capturaHistorial(historialEntregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/mensajeria/obtieneActasER/{anio}/{mes}")]
        public async Task<IActionResult> obtieneActasEntregaRecepcion(int anio, string mes)
        {
            string archivoO = "";
            string fecha = DateTime.Now.ToString("yyyMMddHHmmss");
            string archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Actas";

            if (Directory.Exists(archivoD))
            {
                Directory.Delete(archivoD, true);
            }

            Directory.CreateDirectory(archivoD);

            List<Entregables> actas = await vEntregables.obtieneActasEntregaRecepcion(anio,mes);
            foreach (var ac in actas)
            {
                archivoO = Directory.GetCurrentDirectory() + "\\Entregables\\" + ac.Folio + "\\" + ac.NombreArchivo;
                archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Actas\\" +ac.Tipo+"_"+ ac.Folio+"_"+ac.ClienteEstafeta+".pdf";
                var file = new FileInfo(archivoO);
                var fileD = new FileInfo(archivoD);

                file.CopyTo(archivoD);
            }

            archivoD = Directory.GetCurrentDirectory() + "\\Entregables\\Actas";

            string archivoZip = "Actas_EntregaRecepcion_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(archivoD);
                zip.Comment = "Archivo comprimido el " + System.DateTime.Now.ToString("G");
                zip.Save(Directory.GetCurrentDirectory() + "\\Entregables\\Actas\\" + archivoZip + ".zip");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Entregables\\Actas\\" + archivoZip + ".zip");
            string fileName = archivoZip + ".zip";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
