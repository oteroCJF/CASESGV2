using CedulasEvaluacion.Entities.MContratos;
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
    public class EntregablesContratoController : Controller
    {
        private readonly IRepositorioEntregablesContrato eContrato;
        private readonly IHostingEnvironment environment;

        public EntregablesContratoController(IRepositorioEntregablesContrato evContrato, IHostingEnvironment environment)
        {
            this.eContrato = evContrato ?? throw new ArgumentNullException(nameof(evContrato));
            this.environment = environment;
        }

        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/contrato/InsertarActualizarContrato")]
        public async Task<IActionResult> InsertarActualizarContrato([FromForm] EntregablesContrato entregables)
        {
            int success = await eContrato.InsertarActualizarContrato(entregables);
            if (success != -1)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/contrato/verObligacion/{contrato?}/{nombre?}")]
        public IActionResult verObligacion(string contrato, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\ObligacionesPS\\Contrato_" +contrato+ "\\";
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
        [Route("/contrato/verDocumentacion/{contrato?}/{nombre?}")]
        public IActionResult verDocumentacion(string contrato, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\ObligacionesPS\\Contrato_" + contrato + "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName)+"\\Documentacion\\";
            string pathArchivo = Path.Combine(newPath, nombre);
            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }
        /*Fin del Metodo para adjuntar los entregables*/
    }
}
