using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class OperacionesController : Controller
    {
        private readonly IRepositorioOperaciones vRepositorioOperaciones;

        public OperacionesController(IRepositorioOperaciones iRepositorioOperaciones)
        {
            this.vRepositorioOperaciones = iRepositorioOperaciones ?? throw new ArgumentNullException(nameof(iRepositorioOperaciones));
        }

    }
}
