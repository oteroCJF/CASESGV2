using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MServiciosB;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ServiciosBasicosController : Controller
    {
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioModulos vModulos;
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioEntregablesSB vEntregables;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioServiciosBasicos vBasicos;

        public ServiciosBasicosController(IRepositorioServiciosBasicos iBasicos, IRepositorioCatalogoServicios iCatalogo, IRepositorioInmuebles iInmuebles,
            IRepositorioFacturas iFacturas, IRepositorioPerfiles ivPerfiles, IRepositorioModulos iModulos, IRepositorioEntregablesSB iEntregables)
        {
            this.vBasicos = iBasicos ?? throw new ArgumentNullException(nameof(iBasicos));
            this.vFacturas= iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vCatalogo = iCatalogo ?? throw new ArgumentNullException(nameof(iCatalogo));
            this.vInmuebles = iInmuebles ?? throw new ArgumentNullException(nameof(iInmuebles));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
            this.vModulos= iModulos ?? throw new ArgumentNullException(nameof(iModulos));
            this.vEntregables = iEntregables ?? throw new ArgumentNullException(nameof(iEntregables));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/basicos/index/{servicio}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Anio")] int Anio)
        {
            Models models = new Models();
            Modulos modulos = await vModulos.GetModuloByServicio(servicio);
            models.servicio = await vCatalogo.GetServicioById(servicio);
            int success = await vPerfiles.getPermiso(UserId(), modulos.Nombre, "ver");
            if (success == 1)
            {
                if (Anio != 0)
                {
                    models.Anio = Anio;
                    models.ServicioId = servicio;
                    models.servicios = await vBasicos.GetServicioBasico(Anio, servicio);
                }
                return View(models);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/basicos/nuevaFacturacion/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCaptura(int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloByServicio(servicio)).Nombre, "crear");
            if (success == 1)
            {
                ServicioBasico servicios = new ServicioBasico();
                servicios.ServicioId = servicio;
                servicios.facturas = new List<Facturas>();
                servicios.servicio = await vCatalogo.GetServicioById(servicio);
                return View(servicios);
            }
            return Redirect("/error/denied");
        }

        [Route("/basicos/editarFacturacion/{servicio}/{id}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCaptura(int servicio, int id)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloByServicio(servicio)).Nombre, "actualizar");
            if (success == 1)
            {
                ServicioBasico servicios = await vBasicos.GetServicioBasicoById(id);
                Modulos modulos = await vModulos.GetModuloByServicio(servicio);
                servicios.servicio = await vCatalogo.GetServicioById(servicio);
                servicios.inmuebles = await vInmuebles.inmuebleById(servicios.InmuebleId);
                servicios.facturas = new List<Facturas>();
                servicios.facturas = await vFacturas.getFacturasSB(id,servicio);
                servicios.TotalMontoFactura = vFacturas.obtieneTotalFacturas(servicios.facturas);
                servicios.entregables = new List<Entregables>();
                servicios.entregables = await vEntregables.GetEntregablesSB(servicio);
                return View(servicios);
            }
            return Redirect("/error/denied");
        }

        [Route("/basicos/verfactura/{servicio}/{id}")]
        [HttpGet]
        public async Task<IActionResult> VerFactura(int servicio, int id)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloByServicio(servicio)).Nombre, "ver");
            if (success == 1)
            {
                ServicioBasico servicios = await vBasicos.GetServicioBasicoById(id);
                servicios.servicio = await vCatalogo.GetServicioById(servicio);
                servicios.facturas = await vFacturas.getFacturasSB(id, servicio);
                return View(servicios);
            }
            return Redirect("/error/denied");
        }

        [Route("/basicos/insertaServicioB")]
        [HttpPost]
        public async Task<IActionResult> insertaServicioBasico([FromForm] ServicioBasico servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloById(servicio.ServicioId)).Nombre, "crear");
            if (success == 1)
            {
                servicio.servicio = await vCatalogo.GetServicioById(servicio.ServicioId);
                int insert = await vBasicos.insertaServicioBasico(servicio);
                if(insert != -1 && insert != 0)
                {
                    return Ok(insert);
                }
            }
            return BadRequest(); ;
        }

        [Route("/basicos/actualizaServicioB")]
        [HttpPost]
        public async Task<IActionResult> actualizaServicioBasico([FromForm] ServicioBasico servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloById(servicio.ServicioId)).Nombre, "actualizar");
            if (success == 1)
            {
                int insert = await vBasicos.actualizaServicioBasico(servicio);
                if (insert != -1 && insert != 0)
                {
                    return Ok(insert);
                }
            }
            return BadRequest();
        }

        [Route("/basicos/enviaServicioBasico")]
        [HttpPost]
        public async Task<IActionResult> enviaServicioBasico([FromForm] ServicioBasico servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), (await vModulos.GetModuloById(servicio.ServicioId)).Nombre, "actualizar");
            if (success == 1)
            {
                int insert = await vBasicos.enviaServicioBasico(servicio);
                if (insert != -1 && insert != 0)
                {
                    return Ok(insert);
                }
            }
            return BadRequest();
        }

        [Route("/basicos/eliminaServicioB")]
        [HttpPost]
        public async Task<IActionResult> eliminaServicioBasico([FromBody] ServicioBasico servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(),(await vModulos.GetModuloById(servicio.ServicioId)).Nombre, "eliminar");
            if (success == 1)
            {
                
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }
    }
}
