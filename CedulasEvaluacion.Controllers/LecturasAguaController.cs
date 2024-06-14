using CedulasEvaluacion.Entities.MLecturasAgua;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class LecturasAguaController : Controller
    {
        private readonly IRepositorioLecturasAgua vLectura;
        private readonly IRepositorioEntregablesLecturasAgua eLecturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;

        public LecturasAguaController(IRepositorioLecturasAgua iLectura, IRepositorioEntregablesLecturasAgua iLecturas, IRepositorioInmuebles iInmuebles, 
            IRepositorioUsuarios iUsuario, IRepositorioCatalogoServicios iCatalogo,
            IRepositorioPerfiles iPerfiles)
        {
            this.vLectura= iLectura ?? throw new ArgumentNullException(nameof(iLectura));
            this.eLecturas = iLecturas ?? throw new ArgumentNullException(nameof(iLecturas));
            this.vUsuarios = iUsuario ?? throw new ArgumentNullException(nameof(iUsuario));
            this.vInmuebles = iInmuebles ?? throw new ArgumentNullException(nameof(iInmuebles));
            this.vUsuarios = iUsuario ?? throw new ArgumentNullException(nameof(iUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        [Route("/lectura/index")]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "Anio")] int anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                ModelsLectura models = new ModelsLectura();
                if (anio != 0)
                {
                    models.Anio = anio;
                    models.dashboard = await vLectura.GetLecturas(anio,UserId());
                }
                return View(models);
            }

            return Redirect("/error/denied");
        }

        [Route("/lectura/inmueble/{inmueble}/index")]
        [HttpGet]
        public async Task<IActionResult> Inmueble(int inmueble, [FromQuery(Name = "Anio")] int anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            ModelsLectura models = new ModelsLectura();
            if (success == 1)
            {
                models.Anio = anio;
                models.lectura = await vLectura.GetLecturasByInmueble(anio, inmueble);
                foreach(var inm in models.lectura)
                {
                    inm.inmueble = await vInmuebles.inmuebleById(inm.InmuebleId);
                }
                models.inmueble = await vInmuebles.inmuebleById(inmueble);
                return View(models);
            }

            return Redirect("/error/denied");
        }

        [Route("/lectura/inmueble/{inmueble}/new")]
        [HttpGet]
        public async Task<IActionResult> NuevaLectura(int inmueble, [FromQuery(Name = "Anio")] int anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");

            if (success == 1)
            {
                LecturaAgua lectura = new LecturaAgua();
                lectura.Anio = anio;
                lectura.inmueble = await vInmuebles.inmuebleById(inmueble);
                return View(lectura);
            }

            return Redirect("/error/denied");
        }

        [Route("/lectura/inmueble/{inmueble}/actualizar/{id}")]
        [HttpGet]
        public async Task<IActionResult> NuevaLectura(int inmueble, int id, [FromQuery(Name = "Anio")] int anio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");

            if (success == 1)
            {
                LecturaAgua lectura = await vLectura.GetLecturaById(id);
                lectura.inmueble = await vInmuebles.inmuebleById(lectura.InmuebleId);
                lectura.entregables = await eLecturas.getEntregablesByLectura(id);
                return View(lectura);
            }

            return Redirect("/error/denied");
        }

        [Route("/lectura/inmueble/{inmueble}/revisar/{id}")]
        [HttpGet]
        public async Task<IActionResult> RevisarLectura(int inmueble, int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisar");

            if (success == 1)
            {
                LecturaAgua lectura = await vLectura.GetLecturaById(id);
                lectura.inmueble = await vInmuebles.inmuebleById(lectura.InmuebleId);
                lectura.entregables = await eLecturas.getEntregablesByLectura(id);
                return View(lectura);
            }

            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/lectura/insertaLectura")]
        public async Task<IActionResult> insertaLectura([FromBody] LecturaAgua lectura)
        {
            int success = await vLectura.insertaLectura(lectura);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/lectura/actualizaLectura")]
        public async Task<IActionResult> actualizaLectura([FromBody] LecturaAgua lectura)
        {
            int success = await vLectura.actualizaLectura(lectura);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/lectura/eliminaLectura")]
        public async Task<IActionResult> eliminaLectura(int id)
        {
            int success = await vLectura.eliminaLectura(id);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }


        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Lectura_de_Agua";
        }

    }
}
