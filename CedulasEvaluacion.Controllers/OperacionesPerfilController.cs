using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class OperacionesPerfilController : Controller
    {
        private readonly IRepositorioOperacionesPerfil vRepositorioOpePerfil;

        public OperacionesPerfilController(IRepositorioOperacionesPerfil iRepositorioOpePerfil)
        {
            this.vRepositorioOpePerfil = iRepositorioOpePerfil ?? throw new ArgumentNullException(nameof(iRepositorioOpePerfil));
            
        }

        [HttpPost]
        [Route("/operacionesPerfil/insertaOpPerfil")]
        public async Task<ActionResult> insertarOperacionesPerfil([FromBody] OperacionesPerfil operacionesPerfil)
        {
            int success = 0;
            success = await vRepositorioOpePerfil.insertarOperacionesPerfil(operacionesPerfil);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/perfiles/borraOpPerfil/{perfil?}")]
        public async Task<ActionResult> eliminaOperacionesPerfil(int perfil)
        {   
            int success = 0;
            success = await vRepositorioOpePerfil.eliminaOpPerfil(perfil);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }
    }
}
