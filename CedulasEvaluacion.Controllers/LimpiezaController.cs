﻿using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class LimpiezaController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioCuestionarios vQuestion;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IRepositorioVariables vVariables;
        private readonly IRepositorioFiltrado vFiltrado;

        public LimpiezaController(IRepositorioEvaluacionServicios viCedula, IRepositorioCuestionarios iQuestion, IRepositorioInmuebles iVInmueble,
                                  IRepositorioIncidencias iIncidencias, IRepositorioUsuarios iVUsuario, IRepositorioEntregablesCedula iEntregables,
                                  IRepositorioPerfiles iPerfiles, IRepositorioFacturas iFacturas, IRepositorioVariables iVariables, 
                                  IRepositorioFiltrado iFiltrado)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vQuestion = iQuestion ?? throw new ArgumentNullException(nameof(iQuestion));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vIncidencias = iIncidencias ?? throw new ArgumentNullException(nameof(iIncidencias));
            this.vEntregables = iEntregables ?? throw new ArgumentNullException(nameof(iEntregables));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/limpieza/index/{servicio?}")]
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
                models.administraciones = await vInmuebles.getAdministracionesByServicio(servicio,UserId());
                models.inmuebles = await vInmuebles.getFiltrosInmuebles(UserId(), servicio);
                models.cedulas = await vCedula.GetCedulasEvaluacion(models,UserId());
                return View(models);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/limpieza/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                TempData["Title"] = "\"Limpieza en Áreas Comunes y Oficinas\"";
                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula.ServicioId = servicio;
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/limpieza/new")]
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


        [Route("/limpieza/validaPeriodo")]
        public async Task<IActionResult> validaPeriodo([FromBody] CedulaEvaluacion cedula)
        {
            int valida = await vCedula.VerificaCedula(cedula);
            if (valida == 0)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/limpieza/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                if (cedula.Estatus.Equals("Enviado a DAS") && isEvaluate() == true)
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

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/evaluation")]
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
        [Route("/limpieza/capacitacion")]
        public async Task<IActionResult> guardaCapacitacion([FromBody] List<RespuestasEncuesta> respuestas)
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

        [HttpGet]
        [Route("/limpieza/revision/{id}")]
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
                cedula.incidencias = new ModelsIncidencias();
                cedula.incidencias.iLimpieza = await vIncidencias.getIncidencias(cedula.Id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vCedula.getHistorial(cedula.Id);
                cedula.TotalDeductivas = await vCedula.SumaDeductivas(cedula.Id, cedula.ServicioId) + Convert.ToDecimal(cedula.PenaCalificacion);
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
                if (await vCedula.GetPreguntaCapacitacion(id))
                {
                    return View(cedula);
                }
                else
                {
                    return Redirect("/limpieza/evaluacion/" + id+ Request.QueryString.Value);
                }
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/limpieza/seguimiento/{id}")]
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
                cedula.incidencias = new ModelsIncidencias();
                cedula.incidencias.iLimpieza = await vIncidencias.getIncidencias(cedula.Id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vCedula.getHistorial(cedula.Id);
                cedula.TotalDeductivas = await vCedula.SumaDeductivas(cedula.Id, cedula.ServicioId) + Convert.ToDecimal(cedula.PenaCalificacion);
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
                if (await vCedula.GetPreguntaCapacitacion(id))
                {
                    return View(cedula);
                }
                else
                {
                    return Redirect("/limpieza/evaluacion/" + id);
                }
            }
            return Redirect("/error/denied");
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/sendCedula/{servicio?}/{id?}")]
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

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion cedula)
        {
            int success = 0;
            success = await vCedula.apruebaRechazaCedula(cedula);
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
        [Route("/limpieza/historialLimpieza")]
        public async Task<IActionResult> historialLimpieza([FromBody] HistorialCedulas historialCedulas)
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

        public decimal obtieneTotalBajoDemanda(List<RespuestasAdicionales> respuestasAdicionales)
        {
            decimal total = 0;
            foreach (var bd in respuestasAdicionales)
            {
                if (bd.Pregunta.Equals("EntregableBD"))
                {
                    total += bd.MontoPenalizacion;
                }
            }
            return total;
        }

        [Route("/limpieza/eliminar/{id?}")]
        public async Task<IActionResult> EliminaCedula(int id)
        {
            int excel = await vCedula.EliminaCedula(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Datos del Modulo*/
        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }
        private string modulo()
        {
            return "Limpieza";
        }
        private bool isEvaluate()
        {
            if ((@User.Claims.ElementAt(2).Value).Contains("Evaluador"))
            {
                return true;
            }
            return false;
        }

        private List<string> generateFilterArray(string value)
        {
            List<string> array = new List<string>();
            if (value != null) {
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