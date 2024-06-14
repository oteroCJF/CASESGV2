using CedulasEvaluacion.Entities.MAvisos;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class AvisosCASESGController : Controller
    {
        private readonly IRepositorioAvisosCASESG vAvisos;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;

        public AvisosCASESGController(IRepositorioAvisosCASESG iAvisos, IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles)
        {
            this.vAvisos = iAvisos ?? throw new ArgumentNullException(nameof(iAvisos));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        [Route("/avisos/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                List<AvisosCASESG> avisos= await vAvisos.GetAvisosCASESG();
                return View(avisos);
            }

            return Redirect("/error/denied");
        }

        [Route("/avisos/nuevoAviso")]
        [HttpGet]
        public async Task<IActionResult> NuevoAviso()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                AvisosCASESG avisos= new AvisosCASESG();
                avisos.avisoP = new List<AvisosPerfil>();
                return View(avisos);
            }
            return Redirect("/error/denied");
        }

        [Route("/avisos/editarAviso/{aviso}")]
        [HttpGet]
        public async Task<IActionResult> NuevoAviso(int aviso)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                AvisosCASESG avisos = await vAvisos.GetAvisosCASESGById(aviso);
                avisos.avisoP = await vAvisos.GetPerfilesByAviso(aviso);
                foreach (var pr in avisos.avisoP)
                {
                    pr.perfil = await vPerfiles.getPerfilById(pr.PerfilId);
                }
                return View(avisos);
            }
            return Redirect("/error/denied");
        }

        [Route("/avisos/detalleAviso/{aviso}")]
        [HttpGet]
        public async Task<IActionResult> DetalleAviso(int aviso)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                AvisosCASESG avisos = await vAvisos.GetAvisosCASESGById(aviso);
                avisos.avisoP = await vAvisos.GetPerfilesByAviso(aviso);
                foreach (var pr in avisos.avisoP)
                {
                    pr.perfil = await vPerfiles.getPerfilById(pr.PerfilId);
                }
                return View(avisos);
            }
            return Redirect("/error/denied");
        }

        [Route("/avisos/insertaAviso")]
        [HttpPost]
        public async Task<IActionResult> insertaAviso([FromBody] AvisosCASESG aviso)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                int avisoId = await vAvisos.insertaAvisoCASESG(aviso);
                if (avisoId != 0 && avisoId != -1)
                {
                    int perfiles = await vAvisos.insertaAvisosPerfil(aviso.avisoP, avisoId);
                    if (perfiles == 1)
                    {
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [Route("/avisos/actualizaAviso")]
        [HttpPost]
        public async Task<IActionResult> actualizaAviso([FromBody] AvisosCASESG aviso)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                int avisoId = await vAvisos.actualizaAvisoCASESG(aviso);
                if (avisoId != 0 && avisoId != -1)
                {
                    int perfiles = await vAvisos.insertaAvisosPerfil(aviso.avisoP, aviso.Id);
                    return Ok(perfiles);
                }
            }
            return BadRequest();
        }

        [Route("/avisos/eliminaAviso")]
        [HttpPost]
        public async Task<IActionResult> eliminaAviso([FromBody] AvisosCASESG aviso)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "eliminar");
            if (success == 1)
            {
                int elimino = await vAvisos.eliminaAviso(aviso);
                return Ok(elimino);
            }
            return BadRequest();
        }

        [Route("/avisos/activaAvisos")]
        [HttpPost]
        public async Task<IActionResult> activaAvisos([FromBody] UsuariosAvisos avisosU)
        {
            int visibleId = await vAvisos.insertaUsuarioVisible(avisosU);
            if (visibleId != -1)
            {
                return Ok(visibleId);
            }
            return BadRequest();
        }

        [Route("/avisos/desactivaAvisos")]
        [HttpPost]
        public async Task<IActionResult> desactivaAvisos([FromBody] UsuariosAvisos avisosU)
        {
            int visibleId = await vAvisos.eliminaUsuarioVisible(avisosU);
            if (visibleId != -1)
            {
                return Ok(visibleId);
            }
            return BadRequest();
        }

        [Route("/avisos/eliminaPerfilAviso/{aviso}/{perfil}")]
        [HttpGet]
        public async Task<IActionResult> desactivaAvisos(int aviso, int perfil)
        {
            int delete = await vAvisos.eliminaPerfilAviso(aviso, perfil);
            if (delete != -1)
            {
                return Ok(delete);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Avisos_CASESG";
        }

    }
}
