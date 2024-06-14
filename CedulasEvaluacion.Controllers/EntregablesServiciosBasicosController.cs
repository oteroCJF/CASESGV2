using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class EntregablesServiciosBasicosController : Controller
    {
        private readonly IRepositorioEntregablesSB vEntregables;
        private readonly IHostingEnvironment environment;

        public EntregablesServiciosBasicosController(IRepositorioEntregablesSB iEntregables, IHostingEnvironment environment)
        {
            this.vEntregables = iEntregables ?? throw new ArgumentNullException(nameof(iEntregables));
            this.environment = environment;
        }

        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/entregableSB/insertaEntregableSB")]
        public async Task<IActionResult> insertaEntregableSB([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await vEntregables.insertaEntregableSB(entregables);
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
        
        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/entregableSB/actualizaEntregableSB")]
        public async Task<IActionResult> actualizaEntregableSB([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await vEntregables.actualizaEntregableSB(entregables);
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
        [Route("/entregableSB/getEntregables/{id?}")]
        public async Task<IActionResult> getEntregableSB(int id)
        {
            List<Entregables> entregables = null;
            entregables = await vEntregables.GetEntregablesSB(id);
            string table = "";
            string tipo = "";
            if (entregables != null)
            {
                foreach (var entregable in entregables)
                {
                    table += "<tr>" +
                     "<td>" + (entregable.Tipo.Equals("SAT") ? "Validación del SAT": entregable.Tipo )+ "</td>" +
                     "<td>" + entregable.NombreArchivo + "</td>" +
                     "<td>" + entregable.FechaCreacion.ToString("yyyy-MM-dd") + "</td>" +
                     "<td>" +
                         "<a href='#' class='text-center mr-2 view_file' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo + "' data-tipo ='" + entregable.Tipo + "'>" +
                            "<i class='fas fa-eye text-success'></i>" +
                         "</a>" +
                         "<a href='#' class='text-center mr-2 update_files' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo + "' " +
                            "data-tipo ='" + entregable.Tipo + "' data-comentarios ='" + entregable.Comentarios + "'>" +
                            "<i class='fas fa-pencil text-primary'></i>" +
                         "</a>" +
                         "<a href='#' class='text-center mr-2 delete_files' data-id='" + entregable.Id + "' data-tipo='" + entregable.Tipo + "'>" +
                            "<i class='fas fa-times text-danger'></i>" +
                         "</a>" +
                     "</td>" +
                     "</tr>";
                }
                return Ok(table);
            }
            return NoContent();
        }
        /*FIN del metodo para obtener las inasistencias*/

        [HttpGet]
        [Route("/entregableSB/verArchivo/{folio?}/{nombre?}")]
        public IActionResult archivoProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables SB\\" + folio + "\\";
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
        [Route("/entregableSB/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] Entregables entregable)
        {
            int success = 0;
            success = await vEntregables.eliminaEntregable(entregable);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /*[HttpGet]
        [Route("/entregableSB/buscaEntregable/{id?}/{tipo?}")]
        public async Task<IActionResult> buscaEntregable(int id, string tipo)
        {
            int exists = 0;
            exists = await vEntregables.buscaEntregable(id, tipo);
            if (exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }*/
    }
}
