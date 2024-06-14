using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class PerfilCelularController : Controller
    {
        private readonly IRepositorioPerfilCelular vPCelular;
        private readonly IHostingEnvironment environment;

        public PerfilCelularController(IRepositorioPerfilCelular ivPCelular)
        {
            this.vPCelular= ivPCelular ?? throw new ArgumentNullException(nameof(ivPCelular));
        }

        [Route("/perfilesCelular/getPerfilesCelular")]
        public async Task<IActionResult> GetPerfilesCelular()
        {
            List<PerfilesCelular> perfiles = await vPCelular.GetPerfilesCelular();
            if (perfiles != null)
            {
                return Ok(perfiles);
            }
            return BadRequest();
        }

        [Route("/perfilesCelular/getPerfilCelular/{id?}")]
        public async Task<IActionResult> GetPerfilCelular(int id)
        {
            PerfilesCelular perfil = await vPCelular.GetPerfilCelularById(id);
            if (perfil != null)
            {
                return Ok(perfil);
            }
            return BadRequest();
        }
    }
}
