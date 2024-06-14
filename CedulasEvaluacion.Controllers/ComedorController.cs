using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class ComedorController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioCuestionarios vQuestion;
        private readonly IRepositorioIncidenciasComedor iComedor;
        private readonly IRepositorioEntregablesCedula eComedor;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioFiltrado vFiltrado;

        public ComedorController(IRepositorioEvaluacionServicios iCedula, IRepositorioInmuebles iInmueble, IRepositorioUsuarios iUsuario, 
                                 IRepositorioPerfiles iPerfiles, IRepositorioFacturas iFacturas, IRepositorioVariables iVariables,
                                 IRepositorioIncidenciasComedor iiComedor, IRepositorioEntregablesCedula eeComedor,
                                 IRepositorioCuestionarios iQuestion, IRepositorioFiltrado iFiltrado)
        {
            this.vCedula = iCedula ?? throw new ArgumentNullException(nameof(iCedula));
            this.vQuestion = iQuestion ?? throw new ArgumentNullException(nameof(iQuestion));
            this.iComedor = iiComedor ?? throw new ArgumentNullException(nameof(iiComedor));
            this.eComedor = eeComedor ?? throw new ArgumentNullException(nameof(eeComedor));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iInmueble ?? throw new ArgumentNullException(nameof(iInmueble));
            this.vUsuarios = iUsuario ?? throw new ArgumentNullException(nameof(iUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
        }

        [Route("/comedor/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Anio")] string anio, [FromQuery(Name = "Mes")] string mes, [FromQuery(Name = "Estatus")] string estatus,
            [FromQuery(Name = "Inmueble")] string inmueble, [FromQuery(Name = "TipoServicio")] string tipoServicio, 
            [FromQuery(Name = "Periodo")] string periodo)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                ModelsIndex models = new ModelsIndex();
                models.filtroAnios = generateFilterArray(anio);
                models.filtroMeses = generateFilterArray(mes);
                models.filtroEstatus = generateFilterArray(estatus);
                models.filtroTipoServicio = generateFilterArray(tipoServicio);
                models.filtroInmuebles = generateFilterArray(inmueble);
                models.filtroPeriodo = periodo;
                models.ServicioId = servicio;
                models.anios = await vVariables.GetAniosEvaluacion();
                models.festatus = await vFiltrado.GetEstatusEvaluacion(servicio, UserId());
                models.Meses = await vFiltrado.GetMesesEvaluacion(servicio, UserId());
                models.administraciones = await vInmuebles.getAdministracionesByServicio(servicio, UserId());
                models.inmuebles = await vInmuebles.getComedoresAEvaluar(UserId());
                models.cedulas = await vCedula.GetCedulasEvaluacion(models, UserId());
                return View(models);
            }

            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/comedor/nuevaCedula/{servicio}")]
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
        [Route("/comedor/new")]
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


        [Route("/comedor/validaPeriodo")]
        public async Task<IActionResult> validaPeriodo([FromBody] CedulaEvaluacion cedula)
        {
            int valida = await vCedula.VerificaCedula(cedula);
            if (valida == 0)
            {
                return Ok(valida);
            }
            return BadRequest(valida);
        }

        [Route("/comedor/evaluacion/{id?}")]
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
                cedula.preguntas = await vQuestion.GetCuestionarioByServicio(cedula.ServicioId,id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.horarioComedor = await vVariables.GetHorarioComedor(cedula.InmuebleId, cedula.ServicioId);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/comedor/evaluation")]
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

        [HttpPost]
        [Route("/comedor/sendCedula/{servicio?}/{id?}")]
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
        [Route("/comedor/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion comedor = null;
                comedor = await vCedula.CedulaById(id);
                comedor.URL = Request.QueryString.Value;
                comedor.penalizaciones = await vCedula.GetPenalizacionesByCedula(id, comedor.ServicioId);
                comedor.pestanas = await vVariables.GetPestanasRevisionCedula(comedor.ServicioId,id);
                comedor.preguntas = await vQuestion.GetCuestionarioByServicio(comedor.ServicioId, id);
                comedor.facturas = await vFacturas.getFacturas(id, comedor.ServicioId);
                comedor.TotalMontoFactura = vFacturas.obtieneTotalFacturas(comedor.facturas);
                comedor.inmuebles = await vInmuebles.inmuebleById(comedor.InmuebleId);
                comedor.usuarios = await vUsuarios.getUserById(comedor.UsuarioId);
                comedor.iEntregables = await eComedor.getEntregables(comedor.Id, comedor.ServicioId);
                comedor.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                comedor.incidencias.comedor = await iComedor.GetIncidencias(comedor.Id);
                comedor.RespuestasEncuesta = new List<RespuestasEncuesta>();
                comedor.RespuestasEncuesta = await vCedula.obtieneRespuestas(comedor.Id);
                comedor.historialCedulas = new List<HistorialCedulas>();
                comedor.historialCedulas = await vCedula.getHistorial(comedor.Id);
                comedor.TotalDeductivas = await vCedula.SumaDeductivas(comedor.Id, comedor.ServicioId);
                comedor.TotalPenalizaciones = await vCedula.SumaPenalizaciones(comedor.Id, comedor.ServicioId);
                comedor.historialEntregables = new List<HistorialEntregables>();
                comedor.historialEntregables = await eComedor.getHistorialEntregables(comedor.Id, comedor.ServicioId);
                foreach (var user in comedor.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                foreach (var user in comedor.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(comedor);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/comedor/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion comedor = null;
                comedor = await vCedula.CedulaById(id);
                comedor.URL = Request.QueryString.Value;
                comedor.facturas = await vFacturas.getFacturas(id, comedor.ServicioId);
                comedor.pestanas = await vVariables.GetPestanasRevisionCedula(comedor.ServicioId, id);
                comedor.penalizaciones = await vCedula.GetPenalizacionesByCedula(id, comedor.ServicioId);
                comedor.TotalMontoFactura = vFacturas.obtieneTotalFacturas(comedor.facturas);
                comedor.inmuebles = await vInmuebles.inmuebleById(comedor.InmuebleId);
                comedor.usuarios = await vUsuarios.getUserById(comedor.UsuarioId);
                comedor.iEntregables = await eComedor.getEntregables(comedor.Id, comedor.ServicioId);
                comedor.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                comedor.incidencias.comedor = await iComedor.GetIncidencias(comedor.Id);
                comedor.RespuestasEncuesta = new List<RespuestasEncuesta>();
                comedor.RespuestasEncuesta = await vCedula.obtieneRespuestas(comedor.Id);
                comedor.historialCedulas = new List<HistorialCedulas>();
                comedor.historialCedulas = await vCedula.getHistorial(comedor.Id);
                comedor.TotalDeductivas = await vCedula.SumaDeductivas(comedor.Id, comedor.ServicioId);
                comedor.TotalPenalizaciones = await vCedula.SumaPenalizaciones(comedor.Id, comedor.ServicioId);
                comedor.historialEntregables = new List<HistorialEntregables>();
                comedor.historialEntregables = await eComedor.getHistorialEntregables(comedor.Id, comedor.ServicioId);
                foreach (var user in comedor.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                foreach (var user in comedor.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                comedor.historialEntregables = new List<HistorialEntregables>();
                comedor.historialEntregables = await eComedor.getHistorialEntregables(comedor.Id, comedor.ServicioId);
                foreach (var user in comedor.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                comedor.historialNC = await vCedula.HistorialNotasCredito(id);
                return View(comedor);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/comedor/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion cedula)
        {
            int success = await vCedula.apruebaRechazaCedula(cedula);
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
        [Route("/comedor/estatusNC")]
        public async Task<IActionResult> AutorizarRechazarSNC([FromBody] HistorialNotasCredito cedula)
        {
            int success = await vCedula.autorizarRechazarSNC(cedula);
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
        [Route("/comedor/solicitudRechazoCedula")]
        public async Task<IActionResult> SolicitudRechazoCedula([FromBody] CedulaEvaluacion cedula)
        {
            int success = await vCedula.SolicitudRechazoCedula(cedula);
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
        [Route("/comedor/autorizacionRechazoCedula")]
        public async Task<IActionResult> AutorizacionRechazoCedula([FromBody] CedulaEvaluacion cedula)
        {
            int success = await vCedula.AutorizarRechazoCedula(cedula);
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
        [Route("/comedor/historialComedor")]
        public async Task<IActionResult> historialComedor([FromBody] HistorialCedulas historialCedulas)
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
            return "Comedor";
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
