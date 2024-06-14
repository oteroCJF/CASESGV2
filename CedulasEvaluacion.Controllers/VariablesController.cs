using CedulasEvaluacion.Entities.MVariables;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class VariablesController : Controller
    {
        private readonly IRepositorioVariables vVariables;

        public VariablesController(IRepositorioVariables iVariables)
        {
            this.vVariables = iVariables ?? throw new ArgumentNullException(nameof(iVariables));
        }
        
        [HttpGet]
        [Route("/variables/GetAniosEvaluacion")]
        public async Task<IActionResult> GetAniosEvaluacion()
        {
            List<Variables> anios = await vVariables.GetAniosEvaluacion();
            if (anios != null && anios.Count != 0)
            {
                return Ok(anios);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/variables/GetEntregables/{servicio}/{cedula}")]
        public async Task<IActionResult> GetEntregablesCAE(int servicio, int cedula)
        {
            List<Variables> variables= null;
            variables = await vVariables.GetEntregablesByServicioCAE(servicio,cedula);
            if (variables!= null)
            {
                return Ok(variables);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/variables/getAlcances/{servicio}/{cedula}")]
        public async Task<IActionResult> GetAlcances(int servicio, int cedula)
        {
            List<Variables> variables= null;
            variables = await vVariables.GetListadoAlcances(servicio, cedula);
            if (variables!= null)
            {
                return Ok(variables);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/variables/GetEntregablesCAR/{servicio}/{cedula}")]
        public async Task<IActionResult> GetEntregablesCAR(int servicio, int cedula)
        {
            List<Variables> variables = null;
            variables = await vVariables.GetEntregablesByServicioCAR(servicio, cedula);
            if (variables != null)
            {
                return Ok(variables);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/variables/GetEntregablesUpdate/{servicio}/{tipo}")]
        public async Task<IActionResult> GetEntregablesUpdate(int servicio,string tipo)
        {
            List<Variables> variables = null;
            variables = await vVariables.GetEntregablesUpdateByServicio(servicio,tipo);
            if (variables != null)
            {
                return Ok(variables);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/variables/GetTiposIncidencia/{servicio}")]
        public async Task<IActionResult> GetTiposIncidencias(int servicio)
        {
            List<Variables> tipos = await vVariables.GetTiposIncidenciasByServicio(servicio);
            if (tipos!= null && tipos.Count != 0)
            {
                return Ok(tipos);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/variables/GetTiposIncidencia/{servicio}/{tipo}")]
        public async Task<IActionResult> GetDetallesTiposIncidencias(int servicio, string tipo)
        {
            List<Variables> tipos = await vVariables.GetDetalleIncidenciaByTipo(servicio,tipo);
            if (tipos != null && tipos.Count != 0)
            {
                return Ok(tipos);
            }
            return BadRequest();
        }
    }
}
