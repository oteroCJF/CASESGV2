using CedulasEvaluacion.Entities.MLecturasAgua;
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
    public class EntregablesLecturasAguaController:Controller
    {
        private readonly IRepositorioEntregablesLecturasAgua eLecturas;
        private readonly IHostingEnvironment environment;

        public EntregablesLecturasAguaController(IRepositorioEntregablesLecturasAgua iLecturas, IHostingEnvironment environment)
        {
            this.eLecturas = iLecturas ?? throw new ArgumentNullException(nameof(iLecturas));
            this.environment = environment;
        }

        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/entregableLectura/adjuntaEntregable")]
        public async Task<IActionResult> adjuntaEntregable([FromForm] EntregablesLecturasAgua entregables)
        {
            int success = 0;
            success = await eLecturas.adjuntaEntregable(entregables);
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

        [HttpPost]
        [Route("/entregableLectura/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] EntregablesLecturasAgua entregable)
        {
            int success = 0;
            success = await eLecturas.eliminaEntregables(entregable);
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
        [Route("/entregableLectura/pdf/{folio?}/{nombre?}")]
        public IActionResult archivoProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables Lectura Agua\\" + folio + "\\";
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

        [HttpGet]
        [Route("/entregableLectura/{folio?}/{nombre?}")]
        public IActionResult archivoJPG(string folio, string nombre)
        {
            string folderName = "\\Lecturas Agua\\" + folio + "\\"+nombre;

            return Ok(folderName);
        }
    }
}
