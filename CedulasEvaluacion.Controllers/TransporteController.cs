using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class TransporteController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioEntregablesCedula eTransporte;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioFiltrado vFiltrado;

        public TransporteController(IRepositorioEvaluacionServicios viCedula, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasTransporte iiTransporte, IRepositorioEntregablesCedula eeTransporte,
                                    IRepositorioPerfiles iPerfiles, IRepositorioFacturas iFacturas, IRepositorioVariables iVariables,
                                  IRepositorioFiltrado iFiltrado)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.iTransporte = iiTransporte ?? throw new ArgumentNullException(nameof(iiTransporte));
            this.eTransporte = eeTransporte ?? throw new ArgumentNullException(nameof(eeTransporte));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/transporte/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Anio")] string anio, [FromQuery(Name = "Mes")] string mes, [FromQuery(Name = "Estatus")] string estatus,
            [FromQuery(Name = "Inmueble")] string inmueble, [FromQuery(Name = "administracion")] string administracion)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                ModelsIndex models = new ModelsIndex();
                models.filtroAnios = generateFilterArray(anio);
                models.filtroMeses = generateFilterArray(mes);
                models.filtroEstatus = generateFilterArray(estatus);
                models.filtroAdmins = generateFilterArray(administracion);
                models.filtroInmuebles = generateFilterArray(inmueble);
                models.ServicioId = servicio;
                models.anios = await vVariables.GetAniosEvaluacion();
                models.festatus = await vFiltrado.GetEstatusEvaluacion(servicio, UserId());
                models.Meses = await vFiltrado.GetMesesEvaluacion(servicio, UserId());
                models.administraciones = await vInmuebles.getAdministracionesByServicio(servicio, UserId());
                models.inmuebles = await vInmuebles.getFiltrosInmuebles(UserId(), servicio);
                models.cedulas = await vCedula.GetCedulasEvaluacion(models, UserId());
                return View(models);
            }

            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/transporte/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula.ServicioId = servicio;
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/transporte/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaEvaluacion cedula)
        {
            cedula.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vCedula.insertaCedula(cedula);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }


        [Route("/transporte/validaPeriodo")]
        public async Task<IActionResult> validaPeriodo([FromBody] CedulaEvaluacion cedula)
        {
            int valida = await vCedula.VerificaCedula(cedula);
            if (valida == 0)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/transporte/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                if (cedula.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.URL = Request.QueryString.Value;
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/transporte/evaluation")]
        public async Task<IActionResult> guardaCuestionario([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vCedula.GuardaRespuestas(respuestas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/transporte/sendCedula/{servicio?}/{id?}")]
        public async Task<IActionResult> enviaCedula(int servicio, int id)
        {
            int success = 0;
            success = await vCedula.enviaRespuestas(servicio, id);
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
        [Route("/transporte/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedTran = null;
                cedTran = await vCedula.CedulaById(id);
                cedTran.URL = Request.QueryString.Value;
                cedTran.facturas = await vFacturas.getFacturas(id, cedTran.ServicioId);//
                cedTran.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedTran.facturas);
                cedTran.inmuebles = await vInmuebles.inmuebleById(cedTran.InmuebleId);
                cedTran.usuarios = await vUsuarios.getUserById(cedTran.UsuarioId);
                cedTran.iEntregables = await eTransporte.getEntregables(cedTran.Id,cedTran.ServicioId);
                cedTran.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedTran.incidencias.transporte = await iTransporte.GetIncidencias(cedTran.Id);
                cedTran.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedTran.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedTran.Id);
                cedTran.historialCedulas = new List<HistorialCedulas>();
                cedTran.historialCedulas = await vCedula.getHistorial(cedTran.Id);
                foreach (var user in cedTran.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedTran);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/transporte/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedTran = null;
                cedTran = await vCedula.CedulaById(id);
                cedTran.URL = Request.QueryString.Value;
                cedTran.facturas = await vFacturas.getFacturas(id, cedTran.ServicioId);//
                cedTran.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedTran.facturas);
                cedTran.inmuebles = await vInmuebles.inmuebleById(cedTran.InmuebleId);
                cedTran.usuarios = await vUsuarios.getUserById(cedTran.UsuarioId);
                cedTran.iEntregables = await eTransporte.getEntregables(cedTran.Id, cedTran.ServicioId);
                cedTran.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedTran.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedTran.Id);
                cedTran.historialCedulas = new List<HistorialCedulas>();
                cedTran.historialCedulas = await vCedula.getHistorial(cedTran.Id);
                foreach (var user in cedTran.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedTran.historialEntregables = new List<HistorialEntregables>();
                cedTran.historialEntregables = await eTransporte.getHistorialEntregables(cedTran.Id,cedTran.ServicioId);
                foreach (var user in cedTran.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedTran);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/transporte/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion cedulaTransporte)
        {
            int success = await vCedula.apruebaRechazaCedula(cedulaTransporte);
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
        [Route("/transporte/historialTransporte")]
        public async Task<IActionResult> historialTransporte([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vCedula.capturaHistorial(historialCedulas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /*Datos del Modulo*/
        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Transporte_de_Personal";
        }

        private List<string> generateFilterArray(string value)
        {
            List<string> array = new List<string>();
            if (value != null)
            {
                int values = value.Split(",").Length;
                for (var i = 0; i < values; i++)
                {
                    array.Add(value.Split(",")[i]);
                }
            }
            return array;
        }
    }
}
