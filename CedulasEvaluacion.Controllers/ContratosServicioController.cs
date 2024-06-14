using CedulasEvaluacion.Entities.MContratos;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ContratosServicioController : Controller
    {
        private readonly IRepositorioContratosServicio vContrato;
        private readonly IRepositorioPerfiles vPerfiles;

        public ContratosServicioController(IRepositorioContratosServicio ivContrato, IRepositorioPerfiles ivPerfiles)
        {
            this.vContrato = ivContrato ?? throw new ArgumentNullException(nameof(ivContrato));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
        }

        [HttpGet]
        [Route("/contratos/getContratos/{servicio}")]
        public async Task<IActionResult> GetContratosServicios(int servicio)
        {
            List<ContratosServicio> resultado = await vContrato.GetContratosServicios(servicio);
            if (resultado != null)
            {
                return Ok(resultado);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/contratos/getContrato/{servicio}")]
        public async Task<IActionResult> GetContratoServicioActivo(int servicio)
        {
            ContratosServicio resultado = await vContrato.GetContratoServicioActivo(servicio);
            if (resultado != null)
            {
                return Ok(resultado);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/contratos/insertaContrato")]
        public async Task<IActionResult> InsertaContrato([FromBody] ContratosServicio contratosServicio)
        {
            int insert = await vContrato.InsertaContrato(contratosServicio);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/contratos/actualizaContrato")]
        public async Task<IActionResult> ActualizaContrato([FromBody] ContratosServicio contratosServicio)
        {
            int insert = await vContrato.ActualizaContrato(contratosServicio);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/contrato/InsertarActualizarsDocumentacion")]
        public async Task<IActionResult> InsertarActualizarDocumentacion([FromForm] EntregablesContrato entregables)
        {
            int success = await vContrato.InsertarActualizarDocumentacion(entregables);
            if (success != -1)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/contrato/eliminarDocumentacion")]
        public async Task<IActionResult> EliminarDocumentacion([FromForm] EntregablesContrato entregables)
        {
            int success = await vContrato.eliminaArchivo(entregables);
            if (success != -1)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/contratos/insertaConvenio")]
        public async Task<IActionResult> InsertaConvenio([FromBody] ConveniosContrato convenios)
        {
            int insert = await vContrato.InsertaConvenio(convenios);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/contratos/actualizaConvenio")]
        public async Task<IActionResult> ActualizaConvenio([FromBody] ConveniosContrato convenios)
        {
            int insert = await vContrato.ActualizaConvenio(convenios);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/contratos/eliminarContrato/{id}")]
        public async Task<IActionResult> EliminarContrato(int id)
        {
            int delete = await vContrato.eliminarContrato(id);
            if (delete != -1)
            {
                return Ok(delete);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/contratos/eliminarConvenio/{id}")]
        public async Task<IActionResult> EliminarConvenio(int id)
        {
            int delete = await vContrato.eliminarConvenio(id);
            if (delete != -1)
            {
                return Ok(delete);
            }
            return BadRequest();
        }
    }
}
