using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MFirmantes;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class FirmantesController : Controller
    {
        private readonly IRepositorioFirmantes vFirmas;
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;

        public FirmantesController(IRepositorioFirmantes iFirmantes, IRepositorioInmuebles iInmuebles, IRepositorioUsuarios iUsuario, IRepositorioCatalogoServicios iCatalogo,
            IRepositorioPerfiles iPerfiles)
        {
            this.vFirmas = iFirmantes ?? throw new ArgumentNullException(nameof(iFirmantes));
            this.vCatalogo = iCatalogo ?? throw new ArgumentNullException(nameof(iCatalogo));
            this.vUsuarios = iUsuario ?? throw new ArgumentNullException(nameof(iUsuario));
            this.vInmuebles = iInmuebles ?? throw new ArgumentNullException(nameof(iInmuebles));
            this.vUsuarios = iUsuario ?? throw new ArgumentNullException(nameof(iUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        [Route("/firmantes/index")]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "Servicio")] int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                ModelsFirmantes models = new ModelsFirmantes();
                models.catalogo = await vCatalogo.GetServiciosByUsers(UserId());
                if (servicio != 0)
                {
                    models.firmantes = await vFirmas.GetInmueblesFirmante(servicio,UserId());
                    foreach (var fm in models.firmantes)
                    {
                        fm.inmueble = await vInmuebles.inmuebleById(fm.InmuebleId);
                        fm.servicio = await vCatalogo.GetServicioById(fm.ServicioId);
                        fm.usuario = await vUsuarios.getUserById(fm.UsuarioId);
                    }
                }
                return View(models);
            }

            return Redirect("/error/denied");
        }

        [Route("/firmantes/actualizaFirmante")]
        [HttpPost]
        public async Task<IActionResult> actualizaFirmante([FromBody]FirmantesServicio firmante)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                int update = await vFirmas.actualizaFirmantes(firmante);
                if (update != -1 && update != 0)
                {
                    return Ok(update);
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [Route("/firmantes/obtieneFirmantes/{user}")]
        public async Task<IActionResult> GetFirmantesCedula(int user)
        {
            List<Usuarios> usuarios = await vFirmas.GetUsuariosByAdministracion(user);
            if (usuarios != null)
            {
                return Ok(usuarios);
            }
            return BadRequest();
        }

        [Route("/firmantes/verificaFirmante/{tipo}/{inmueble}/{servicio}/{cedula}")]
        public async Task<IActionResult> VerificaFirmante(string tipo, int inmueble, int servicio, int cedula)
        {
            int exists = await vFirmas.GetVerificaFirmantes(tipo, inmueble, servicio, cedula);
            if (exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/firmantes/insertaFirmante")]
        public async Task<IActionResult> insertaFirmante([FromBody] FirmantesServicio firmante)
        {
            int exists = await vFirmas.insertaFirmante(firmante);
            if (exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Firmantes";
        }
    }
}