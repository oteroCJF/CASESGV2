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
    public class ConvencionalController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioEntregablesCedula eConvencional;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioFiltrado vFiltrado;

        public ConvencionalController(IRepositorioEvaluacionServicios viCedula, IRepositorioIncidenciasConvencional ivConvencional, IRepositorioEntregablesCedula ieConvencional,
                                      IRepositorioFacturas iFacturas, IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IRepositorioInmuebles iVInmueble, IRepositorioVariables iVariables,
                                  IRepositorioFiltrado iFiltrado)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.iConvencional = ivConvencional ?? throw new ArgumentNullException(nameof(ivConvencional));
            this.eConvencional = ieConvencional ?? throw new ArgumentNullException(nameof(ieConvencional));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
        }



        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/telConvencional/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Anio")] string anio, [FromQuery(Name = "Mes")] string mes, [FromQuery(Name = "Estatus")] string estatus)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                ModelsIndex models = new ModelsIndex();
                models.filtroAnios = generateFilterArray(anio);
                models.filtroMeses = generateFilterArray(mes);
                models.filtroEstatus = generateFilterArray(estatus);
                models.ServicioId = servicio;
                models.anios = await vVariables.GetAniosEvaluacion();
                models.festatus = await vFiltrado.GetEstatusEvaluacion(servicio, UserId());
                models.Meses = await vFiltrado.GetMesesEvaluacion(servicio, UserId());
                models.cedulas = await vCedula.GetCedulasEvaluacion(models, UserId());
                return View(models);
            }

            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/telConvencional/nuevaCedula/{servicio}")]
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
        [Route("/telConvencional/new")]
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


        [Route("/telConvencional/validaPeriodo")]
        public async Task<IActionResult> validaPeriodo([FromBody] CedulaEvaluacion cedula)
        {
            int valida = await vCedula.VerificaCedula(cedula);
            if (valida == 0)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/telConvencional/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                /*if (cedula.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }*/
                cedula.URL = Request.QueryString.Value;
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/telConvencional/evaluation")]
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
        [Route("/telConvencional/sendCedula/{servicio?}/{id?}")]
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


        [Route("/telConvencional/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasConvencional(int id)
        {
            CedulaEvaluacion telefoniaConvencional = await vCedula.CedulaById(id);
            telefoniaConvencional.URL = Request.QueryString.Value;
            telefoniaConvencional.incidencias = new Entities.MIncidencias.ModelsIncidencias();
            telefoniaConvencional.incidencias.convencional = await iConvencional.getIncidenciasConvencional(id);
            return View(telefoniaConvencional);
        }

        [HttpGet]
        [Route("/telConvencional/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion telCel = null;
                telCel = await vCedula.CedulaById(id);
                telCel.URL = Request.QueryString.Value;
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                telCel.incidencias.contratacion = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "contratacion_instalacion");
                telCel.incidencias.cableado = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cableado");
                telCel.incidencias.entregaAparato = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "entregaAparato");
                telCel.incidencias.cambioDomicilio = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cambioDomicilio");
                telCel.incidencias.reubicacion = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "reubicacion");
                telCel.incidencias.identificador = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "identificadorLlamadas");
                telCel.incidencias.instalaciónTroncal = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "troncales");
                telCel.incidencias.contratacionInternet = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "internet");
                telCel.incidencias.habilitacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "serviciosTelefonia");
                telCel.incidencias.cancelacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cancelacion");
                telCel.incidencias.reporteFallas = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "reportesFallas");
                telCel.iEntregables = await eConvencional.getEntregables(telCel.Id, telCel.ServicioId);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCedula.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vCedula.getHistorial(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/telConvencional/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion telCel = null;
                telCel = await vCedula.CedulaById(id);
                telCel.URL = Request.QueryString.Value;
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.iEntregables = await eConvencional.getEntregables(telCel.Id, telCel.ServicioId);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCedula.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vCedula.getHistorial(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                telCel.historialEntregables = new List<HistorialEntregables>();
                telCel.historialEntregables = await eConvencional.getHistorialEntregables(telCel.Id, telCel.ServicioId);
                foreach (var user in telCel.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/telConvencional/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion telefoniaConvencional)
        {
            int success = await vCedula.apruebaRechazaCedula(telefoniaConvencional);
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
        [Route("/telConvencional/historialConvencional")]
        public async Task<IActionResult> historialConvencional([FromBody] HistorialCedulas historialCedulas)
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
            return "Telefonia_Convencional";
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
