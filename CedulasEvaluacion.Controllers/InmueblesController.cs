using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CASESGCedulasEvaluacion.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;

        public InmueblesController(IRepositorioInmuebles iVinmuebles, IRepositorioPerfiles iRepositorioPerfiles)
        {
            this.vInmuebles = iVinmuebles ?? throw new ArgumentNullException(nameof(iVinmuebles));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
        }

        //Listado de todos las Administraciones
        [Route("/inmuebles/index")]
        public async Task<ActionResult<IEnumerable>> Index()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(),"ver");
            if (success == 1)
            {
                List<Inmueble> resultado = new List<Inmueble>();
                resultado = await vInmuebles.getAdministraciones();
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo que redirecciona a la vista DetalleAdministracion
        [Route("/inmuebles/detalleAdministracion/{id?}")]
        public async Task<ActionResult<IEnumerable>> DetalleAdministracion(int id)
        {
            var inmueble = await vInmuebles.inmuebleById(id);
            return View(inmueble);
        }

        //Captura de Nuevo Inmueble/Administracion (Vista)
        [Route("/inmuebles/nuevaAdministracion")]
        public async Task<ActionResult> NuevoInmueble()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(),modulo(), "crear");
            if (success == 1)
            {
                Inmueble inmueble = new Inmueble();
                return View(inmueble);
            }
            return Redirect("/error/denied");
        }

        //Editar Inmueble (Vista)
        [Route("/inmuebles/editar/administracion/{id?}")]
        public async Task<ActionResult<IEnumerable>> EditarInmueble(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(),modulo(), "actualizar");
            if (success == 1)
            {
                var inmueble = await vInmuebles.inmuebleById(id);
                return View(inmueble);
            }
            return Redirect("/error/denied");
        }

        /*Peticiones HTTP API's*/

        //Peticion GET Administraciones
        [HttpGet]
        [Route("/inmuebles/getAdministraciones")]
        public async Task<ActionResult<IEnumerable>> obtieneAdministraciones()
        {
            List<Inmueble> resultado = new List<Inmueble>();
            resultado = await vInmuebles.getAdministraciones();
            return Ok(resultado);
        }

        [HttpGet]
        [Route("/inmuebles/getAdministracionesByServicio/{servicio}")]
        public async Task<ActionResult<IEnumerable>> GetAdministracionesByServicio(int servicio, int usuario)
        {
            List<Inmueble> resultado = await vInmuebles.getAdministracionesByServicio(servicio,usuario);
            return Ok(resultado);
        }

        [HttpGet]
        [Route("/inmuebles/getInmuebles")]
        public async Task<ActionResult<IEnumerable>> obtieneInmuebles()
        {
            List<Inmueble> resultado = new List<Inmueble>();
            resultado = await vInmuebles.getInmuebles();
            return Ok(resultado);
        }

        //Peticion POST para editar el inmueble
        [HttpPost]
        [Route("/inmuebles/actualizarInmueble")]
        public async Task<ActionResult<IEnumerable>> actualizarInmueble([FromBody] Inmueble inmueble)
        {
            var update = await vInmuebles.updateAdmin(inmueble);
            if (update != 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        //Peticion POST para agregar direcciones personalizadas
        [HttpPost]
        [Route("/inmuebles/nuevaDireccion")]
        public async Task<ActionResult<IEnumerable>> insertaDireccionBM(Inmueble inmueble)
        {
            int direccion = await vInmuebles.insertaDireccionBM(inmueble);
            if (direccion != 0)
            {
                return Ok(direccion);
            }
            return BadRequest();
        }

        //Peticion GET Administraciones
        [HttpGet]
        [Route("/inmuebles/getDireccionesBM")]
        public async Task<ActionResult<IEnumerable>> getDireccionesBM()
        {
            List<Inmueble> resultado = new List<Inmueble>();
            resultado = await vInmuebles.getDireccionesAlternasBM();
            return Ok(resultado);
        }


        //Peticion GET para eliminar una administracion
        [HttpGet("/delete/administrator/{id?}")]
        public async Task<ActionResult<IEnumerable>> delete(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(),modulo(), "eliminar");
            if (success == 1)
            {
                var delete = await vInmuebles.deleteAdmin(id);
                if (delete == 1)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }


        //Peticion GET para traer los inmuebles por administracion
        [HttpGet]
        [Route("/getInmueblesAdmin/{admin?}")]
        public async Task<IActionResult> getInmueblesUser(int admin)
        {
            List<Inmueble> inmueble = null;
            inmueble = await vInmuebles.getInmueblesByAdmin(admin);
            if (inmueble != null) {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        //Peticion GET para traer los inmuebles por administracion
        [HttpGet]
        [Route("/inmuebles/getEstadosRM")]
        public async Task<IActionResult> getEstadosRM()
        {
            List<Inmueble> inmueble = null;
            inmueble = await vInmuebles.getEstadosRM();
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        //Peticion GET para traer los inmuebles por administracion y los devuelve en un table para insertar
        [HttpGet]
        [Route("/getTableInmueblesAdmin/{admin?}")]
        public async Task<IActionResult> getTableInmueblesUser(int admin)
        {
            List<Inmueble> inmueble = null;
            inmueble = await vInmuebles.getInmueblesByAdmin(admin);
            if (inmueble != null)
            {
                return Ok(tablaInmuebles(inmueble));
            }
            return BadRequest();
        }

        public string tablaInmuebles(List<Inmueble> inmueble)
        {
            string table = "";
            int i = 0;
            if (inmueble.Count != 0)
            {
                foreach (var inmUsr in inmueble)
                {
                    i++;
                    table +=
                    "<tr>" +
                        "<td>" + inmUsr.Id + "</td>" +
                        "<td>" + (inmUsr.Clave) + "</td>" +
                        "<td>" + inmUsr.Nombre + "</td>" +
                        "<td>" + inmUsr.Direccion+ "</td>" +
                    "</tr>";
                }
            }
            return table;
        }

        //Peticion GET para traer los inmuebles por administracion
        [HttpGet]
        [Route("/inmuebles/evaluar")]
        public async Task<IActionResult> getInmueblesEvaluar()
        {
            List<Inmueble> inmueble = null;
            int user = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            inmueble = await vInmuebles.getInmueblesAEvaluar(user);
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/inmuebles/comedores/evaluar")]
        public async Task<IActionResult> getComedoresEvaluar()
        {
            List<Inmueble> inmueble = null;
            int user = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            inmueble = await vInmuebles.getComedoresAEvaluar(user);
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/inmuebles/filtros/{servicio}")]
        public async Task<IActionResult> getFiltrosInmuebles(int servicio)
        {
            List<Inmueble> inmueble = null;
            int user = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            inmueble = await vInmuebles.getFiltrosInmuebles(UserId(),servicio);
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Administraciones";
        }
    }
}
