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
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuarios vRepositorioUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioAreas vRepositorioAreas;

        public UsuariosController(IRepositorioUsuarios iRepositorioUsuarios, IRepositorioPerfiles iRepositorioPerfiles, IRepositorioAreas iRepositorioAreas)
        {
            this.vRepositorioUsuarios = iRepositorioUsuarios ?? throw new ArgumentNullException(nameof(iRepositorioUsuarios));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.vRepositorioAreas = iRepositorioAreas ?? throw new ArgumentNullException(nameof(iRepositorioAreas));

        }

        [Route("/usuarios/index")]
        public async Task<IActionResult> index(List<Usuarios> usuarios)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                usuarios = await vRepositorioUsuarios.getUsuarios();
                return View(usuarios);
            }
            return Redirect("/error/denied");
        }

        //Obtenemos usuarios por Id
        [Route("/detailUser/{id?}")]
        public async Task<IActionResult> DetalleUsuario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                Usuarios usuarios = null;
                usuarios = await vRepositorioUsuarios.getUserById(id);
                usuarios.areas = await vRepositorioAreas.getAreasById(usuarios.AreaId);
                if (usuarios != null)
                {
                    return View(usuarios);
                }
                return NoContent();
            }
            return Redirect("/error/denied");
        }
        
        //Obtenemos los inmuebles asignados a el usuario
        [HttpGet]
        [Route("/getInmueblesUser/{user?}")]
        public async Task<IActionResult> getInmueblesUser(int user)
        {
            List<InmueblesUsuarios> inUsr = null;
            inUsr = await vRepositorioUsuarios.getInmueblesUsuario(user);
            return Ok(inUsr);
        }

        //asigna administraciones a los usuarios
        [HttpPost]
        [Route("/usuarios/asignaAdmins")]
        public async Task<IActionResult> asignaAdministraciones([FromBody]List<InmueblesUsuarios> inmueblesUsuarios)
        {
            int inUsr = 0;
            inUsr = await vRepositorioUsuarios.insertaAdminByUser(inmueblesUsuarios);
            if (inUsr != -1) {
                return Ok(inUsr);
            }
            return BadRequest();
        }

        //asignamos el perfil al usuario
        [HttpPost]
        [Route("/usuarios/asignaPerfil")]
        public async Task<ActionResult> asignaPerfil([FromBody] List<PerfilesUsuario> perfilesUsuario)
        {
            int success = 0;
            success = await vRepositorioUsuarios.asignaPerfil(perfilesUsuario);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        //elimina administraciones a los usuarios
        [HttpGet]
        [Route("/usuarios/EliminaAdministracion/{user?}/{administracion?}")]
        public async Task<IActionResult> eliminaAdministracion(int user, int administracion)
        {
            int inUsr = await vRepositorioUsuarios.EliminaAdminByUser(user,administracion);
            if (inUsr != 0)
            {
                return Ok(inUsr);
            }
            return BadRequest();
        }

        //asigna administraciones a los usuarios
        [HttpPost]
        [Route("/usuarios/actualizaEmail")]
        public async Task<IActionResult> actualizaEmail([FromBody]Usuarios usuarios)
        {
            int inUsr = 0;
            inUsr = await vRepositorioUsuarios.actualizaCorreoElectronico(usuarios);
            if (inUsr != 0)
            {
                return Ok(inUsr);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Usuarios";
        }
    }
}
