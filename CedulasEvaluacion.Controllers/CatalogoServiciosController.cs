using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MContratos;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class CatalogoServiciosController:Controller
    {
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioContratosServicio vContrato;
        private readonly IRepositorioEntregablesContrato eContrato;
        private readonly IRepositorioPerfiles vPerfiles;

        public CatalogoServiciosController(IRepositorioCatalogoServicios ivCatalogo, IRepositorioContratosServicio ivContrato, 
            IRepositorioPerfiles ivPerfiles, IRepositorioEntregablesContrato eeContrato)
        {
            this.vCatalogo = ivCatalogo ?? throw new ArgumentNullException(nameof(ivCatalogo));
            this.vContrato = ivContrato ?? throw new ArgumentNullException(nameof(ivContrato));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
            this.eContrato = eeContrato ?? throw new ArgumentNullException(nameof(eeContrato));
        }

        [HttpGet]
        [Route("/catalogo/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<DashboardCS> resultado = await vCatalogo.GetDashBoard();
                if (resultado != null)
                {
                    return View(resultado);
                }
                return NotFound();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/detalleServicio/{servicio}")]
        public async Task<IActionResult> DetalleServicio(int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                ModelsCatalogo models = new ModelsCatalogo();
                models.servicio = await vCatalogo.GetServicioById(servicio);
                models.contratos = await vContrato.GetContratosServicios(servicio);
                models.contrato = await vContrato.GetContratoServicioActivo(servicio);
                if (models != null)
                {
                    return View(models);
                }
                return NotFound();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/detalleContrato/{id}")]
        public async Task<IActionResult> DetalleContrato(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                ModelsCatalogo models = new ModelsCatalogo();
                models.contrato = await vContrato.GetContratoServicioById(id);
                models.contrato.convenios = await vContrato.GetConveniosByContrato(id);
                models.entregables = await eContrato.GetEntregablesCS(id);
                models.servicio = await vCatalogo.GetServicioById(models.contrato.ServicioId);
                if (models != null)
                {
                    return View(models);
                }
                return NotFound();
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/nuevaObligacion/{contrato}")]
        public async Task<IActionResult> CrearObligacion(int contrato)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                EntregablesContrato entregable = new EntregablesContrato();
                entregable.ContratoId = contrato;
                entregable.contrato = await vContrato.GetContratoServicioById(contrato);
                entregable.servicio = await vCatalogo.GetServicioById(entregable.contrato.ServicioId);
                return View("NuevaObligacion",entregable);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/editarObligacion/{id}")]
        public async Task<IActionResult> EditarObligacion(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                EntregablesContrato entregable = await eContrato.GetEntregableCsById(id);
                entregable.contrato = await vContrato.GetContratoServicioById(entregable.ContratoId);
                entregable.servicio = await vCatalogo.GetServicioById(entregable.contrato.ServicioId);
                return View("NuevaObligacion", entregable);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/verObligacion/{id}")]
        public async Task<IActionResult> VerObligacion(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                EntregablesContrato entregable = await eContrato.GetEntregableCsById(id);
                entregable.contrato = await vContrato.GetContratoServicioById(entregable.ContratoId);
                return View(entregable);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/catalogo/servicios")]
        public async Task<IActionResult> GetCatalogoServicios()
        {
            List<CatalogoServicios> resultado = await vCatalogo.GetCatalogoServicios();
            if (resultado != null) {
                return Ok(resultado);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Contratos_DAS";
        }

    }
}
