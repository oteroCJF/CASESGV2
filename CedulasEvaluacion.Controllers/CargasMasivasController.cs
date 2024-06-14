using CedulasEvaluacion.Entities.MCargasMasivas;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class CargasMasivasController : Controller
    {
        private readonly IRepositorioCargasMasivas vCargas;

        public CargasMasivasController(IRepositorioCargasMasivas iCargas)
        {
            this.vCargas= iCargas?? throw new ArgumentNullException(nameof(iCargas));
        }

        [HttpGet]
        [Route("/cargasMasivas/index")]
        public async Task<IActionResult> Index()
        {
            List<CargasMasivas> cm = await vCargas.getCargasMasivas();
            return View(cm);
        }

        [HttpGet]
        [Route("/cargasMasivas/nuevaCarga/{carga}")]
        public async Task<IActionResult> NuevaCarga(int carga)
        {
            CargasMasivas cm = await vCargas.getCargaMasivaById(carga);
            return View(cm);
        }

        [HttpGet]
        [Route("/cargasMasivas/revisarCarga/{carga}")]
        public async Task<IActionResult> RevisarCarga(int carga)
        {
            CargasMasivas cm = await vCargas.getCargaMasivaById(carga);
            cm.facturas = await vCargas.getFacturas(carga);
            return View(cm);
        }

        [HttpPost]
        [Route("/cargasMasivas/insertaCarga")]
        public async Task<IActionResult> NuevaCarga([FromBody] CargasMasivas carga)
        {
            int success = await vCargas.insertaCargaMsiva(carga);
            if(success != -1)
            {
                return Ok(success);
            }
            return BadRequest(success);
        }

        [HttpPost]
        [Route("/cargasMasivas/insertaFactura")]
        public async Task<IActionResult> cargaMasiva([FromForm] Facturas facturas)
        {
            int success = await vCargas.insertaConceptoFacturas(facturas);
            if (success == 1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/cargasMasivas/procesarCarga/{carga}/{tipo}")]
        public async Task<IActionResult> procesarArchivos(int carga, string tipo)
        {
            int complete = await vCargas.procesarArchivos(carga, tipo);
            if (complete != -1)
            {
                return Ok(complete);
            }

            return BadRequest(complete);
        }

    }
}
