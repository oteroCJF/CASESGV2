using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class PerfilesController : Controller
    {
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioModulos vRepositorioModulos;
        private readonly IRepositorioOperaciones vRepositorioOperaciones;
        private readonly IRepositorioOperacionesPerfil vRepositorioOpePerfil;
        

        public PerfilesController(IRepositorioPerfiles iRepositorioPerfiles, IRepositorioModulos iRepositorioModulos, IRepositorioOperaciones iRepositorioOperaciones,
                                  IRepositorioOperacionesPerfil iRepositorioOpePerfil)
        {
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.vRepositorioModulos = iRepositorioModulos ?? throw new ArgumentNullException(nameof(iRepositorioModulos));
            this.vRepositorioOperaciones = iRepositorioOperaciones ?? throw new ArgumentNullException(nameof(iRepositorioOperaciones));
            this.vRepositorioOpePerfil = iRepositorioOpePerfil ?? throw new ArgumentNullException(nameof(iRepositorioOpePerfil));
        }

        [Route("/perfiles/index")]
        public async Task<IActionResult> index(List<Perfiles> perfiles)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                perfiles = await vRepositorioPerfiles.getPerfiles();
                return View(perfiles);
            }
            return Redirect("/error/denied");
        }

        [Route("/perfiles/getPerfiles")]
        public async Task<IActionResult> getPerfiles()
        {
            List<Perfiles> perfiles = null;
            perfiles = await vRepositorioPerfiles.getPerfiles();
            if (perfiles != null) {
                return Ok(perfiles);
            }
            return BadRequest();
        }

        [Route("/perfiles/new")]
        public async Task<IActionResult> NuevoPerfil(Perfiles perfiles)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                perfiles = new Perfiles();
                perfiles.Modulos = await vRepositorioModulos.getModulos();
                perfiles.Operaciones = await vRepositorioOperaciones.getOperaciones();

            return View(perfiles);
            }
            return Redirect("/error/denied");
        }

        [Route("/perfiles/edit/{id?}")]
        public async Task<IActionResult> EditarPerfil(int id)
        {
            int success = 1;// await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                Perfiles perfiles = new Perfiles();
                perfiles = await vRepositorioPerfiles.getPerfilById(id);
                perfiles.Modulos = await vRepositorioModulos.getModulos();
                perfiles.Operaciones = await vRepositorioOperaciones.getOperaciones();
                perfiles.opPerfil = await vRepositorioOpePerfil.getOperacionesByPerfil(id);

                return View(perfiles);
            }
            return Redirect("/error/denied");
        }

        //insertamos el perfil
        [HttpPost]
        [Route("/perfiles/insertaPerfil")]
        public async Task<ActionResult> insertaPerfil([FromBody] Perfiles perfiles)
        {
            int success = 0;
            success = await vRepositorioPerfiles.insertarPerfil(perfiles);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        //actualizamos el perfil
        [HttpPost]
        [Route("/perfiles/actualizaPerfil")]
        public async Task<ActionResult> actualizaPerfil([FromBody] Perfiles perfiles)
        {
            int success = 0;
            success = await vRepositorioPerfiles.actualizaPerfil(perfiles);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        /*Obtenemos la respuesta para accesar al modulo*/
        [HttpGet]
        [Route("/perfiles/permiso/{operacion?}/{modulo?}")]
        public async Task<int> obtienePermiso(string operacion,string modulo)
        {
            int permit = await vRepositorioPerfiles.getPermiso(UserId(), modulo, operacion);
            if (permit == 1)
            {
                return 1;
            }
            return -1;
        }

        //Peticion GET para traer los inmuebles por administracion
        [HttpGet]
        [Route("/perfiles/getPerfileUser/{user?}")]
        public async Task<IActionResult> getPerfilesByUser(int user)
        {
            List<Perfiles> perfiles = null;
            perfiles = await vRepositorioPerfiles.getPerfilesByUser(user);
            if (perfiles != null)
            {
                return Ok(perfiles);
            }
            return BadRequest();
        }

        
        [HttpGet]
        [Route("/perfiles/eliminaPerfilByUser/{id?}/{user?}")]
        public async Task<IActionResult> eliminaPerfilByUser(int id,int user)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                int inUsr = 0;
                inUsr = await vRepositorioPerfiles.eliminaPerfilByUser(id, user);
                if (inUsr != 0)
                {
                    return Ok(inUsr);
                }
            }
            return BadRequest();
        }
        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Perfiles";
        }

    }
}
