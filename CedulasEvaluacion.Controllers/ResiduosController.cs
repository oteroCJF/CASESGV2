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
    public class ResiduosController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioCuestionarios vQuestion;
        private readonly IRepositorioIncidenciasResiduos vIncidencias;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioFiltrado vFiltrado;

        public ResiduosController(IRepositorioEvaluacionServicios viCedula, IRepositorioCuestionarios iQuestion, IRepositorioIncidenciasResiduos ivResiduos, 
                                    IRepositorioEntregablesCedula ieResiduos, IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, 
                                    IRepositorioInmuebles iVInmueble, IRepositorioFacturas iFacturas, IRepositorioVariables iVariables, 
                                    IRepositorioFiltrado iFiltrado)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vQuestion = iQuestion ?? throw new ArgumentNullException(nameof(iQuestion));
            this.vIncidencias = ivResiduos ?? throw new ArgumentNullException(nameof(ivResiduos));
            this.vEntregables = ieResiduos ?? throw new ArgumentNullException(nameof(ieResiduos));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/residuos/index/{servicio?}")]
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
        [Route("/residuos/nuevaCedula/{servicio}")]
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
        [Route("/residuos/new")]
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


        [Route("/residuos/validaPeriodo")]
        public async Task<IActionResult> validaPeriodo([FromBody] CedulaEvaluacion cedula)
        {
            int valida = await vCedula.VerificaCedula(cedula);
            if (valida == 0)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/residuos/evaluacion/{id?}")]
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
                cedula.URL = Request.QueryString.Value;
                cedula.preguntas = await vQuestion.GetCuestionarioByServicio(cedula.ServicioId, id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/residuos/evaluation")]
        public async Task<IActionResult> guardaCuestionario([FromBody] List<RespuestasEncuesta> respuestas)
        {
            if (await vCedula.GetEstatusCedula(respuestas[0].CedulaEvaluacionId))
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
            return NotFound();
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/residuos/sendCedula/{servicio?}/{id?}")]
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
        [Route("/residuos/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedula = null;
                cedula = await vCedula.CedulaById(id);
                cedula.URL = Request.QueryString.Value;
                cedula.penalizaciones = await vCedula.GetPenalizacionesByCedula(id, cedula.ServicioId);
                cedula.pestanas = await vVariables.GetPestanasRevisionCedula(cedula.ServicioId, id);
                cedula.preguntas = await vQuestion.GetCuestionarioByServicio(cedula.ServicioId, id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(cedula.Id, cedula.ServicioId);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedula.incidencias.residuos = await vIncidencias.getIncidencias(cedula.Id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vCedula.getHistorial(cedula.Id);
                cedula.TotalDeductivas = await vCedula.SumaDeductivas(cedula.Id, cedula.ServicioId);
                cedula.TotalPenalizaciones = await vCedula.SumaPenalizaciones(cedula.Id, cedula.ServicioId);
                cedula.historialEntregables = new List<HistorialEntregables>();
                cedula.historialEntregables = await vEntregables.getHistorialEntregables(cedula.Id, cedula.ServicioId);
                foreach (var user in cedula.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                foreach (var user in cedula.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/residuos/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedula = null;
                cedula = await vCedula.CedulaById(id);
                cedula.URL = Request.QueryString.Value;
                cedula.penalizaciones = await vCedula.GetPenalizacionesByCedula(id, cedula.ServicioId);
                cedula.pestanas = await vVariables.GetPestanasRevisionCedula(cedula.ServicioId, id);
                cedula.preguntas = await vQuestion.GetCuestionarioByServicio(cedula.ServicioId, id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(cedula.Id, cedula.ServicioId);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedula.incidencias.residuos = await vIncidencias.getIncidencias(cedula.Id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vCedula.getHistorial(cedula.Id);
                cedula.TotalDeductivas = await vCedula.SumaDeductivas(cedula.Id, cedula.ServicioId);
                cedula.TotalPenalizaciones = await vCedula.SumaPenalizaciones(cedula.Id, cedula.ServicioId);
                cedula.historialEntregables = new List<HistorialEntregables>();
                cedula.historialEntregables = await vEntregables.getHistorialEntregables(cedula.Id, cedula.ServicioId);
                foreach (var user in cedula.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                foreach (var user in cedula.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/residuos/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion residuos)
        {
            int success = await vCedula.apruebaRechazaCedula(residuos);
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
        [Route("/residuos/historialResiduos")]
        public async Task<IActionResult> historialResiduos([FromBody] HistorialCedulas historialCedulas)
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

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "RPBI";
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
