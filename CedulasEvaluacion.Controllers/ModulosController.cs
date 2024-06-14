using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class ModulosController : Controller
    {
        private readonly IRepositorioModulos vRepositorioModulos;

        public ModulosController(IRepositorioModulos iRepositorioModulos)
        {
            this.vRepositorioModulos = iRepositorioModulos ?? throw new ArgumentNullException(nameof(iRepositorioModulos));
        }
    }
}
