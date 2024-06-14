using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class FiltradoController : Controller
    {
        private readonly IRepositorioFiltrado vFiltrado;

        public FiltradoController(IRepositorioFiltrado iFiltrado)
        {
            this.vFiltrado = iFiltrado ?? throw new ArgumentNullException(nameof(iFiltrado));
        }

        [HttpGet]
        [Route("/filtrado/getEstatusEvaluacion/{anio}/{servicio}")]
        public async Task<IActionResult> GetEstatusEvaluacion(int servicio)
        {
            List<CedulaEvaluacion> estatus = await vFiltrado.GetEstatusEvaluacion(servicio, UserId());
            if (estatus != null && estatus.Count != 0)
            {
                return Ok(estatus);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

    }
}
