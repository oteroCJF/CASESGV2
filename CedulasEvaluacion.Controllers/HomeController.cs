using CedulasEvaluacion.Entities.Login;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ServiceReference1;
using Newtonsoft.Json;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MHome;
using CASESGCedulasEvaluacion.Models;

namespace CASESGCedulasEvaluacion.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositorioLogin vRepositorioLogin;
        private readonly IRepositorioAvisosCASESG vAvisos;
        private readonly IRepositorioUsuarios vRepositorioUsuarios;

        public HomeController(IRepositorioLogin iRepositorioLogin, IRepositorioUsuarios iRepositorioUsuarios, IRepositorioAvisosCASESG iAvisos)
        {
            this.vAvisos= iAvisos ?? throw new ArgumentNullException(nameof(iAvisos));
            this.vRepositorioLogin = iRepositorioLogin ?? throw new ArgumentNullException(nameof(iRepositorioLogin));
            this.vRepositorioUsuarios = iRepositorioUsuarios ?? throw new ArgumentNullException(nameof(iRepositorioUsuarios));
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (!User.Claims.ElementAt(5).Value.Equals("[]")) {
                ModelsDash models = new ModelsDash();
                models.dashboards = await vRepositorioLogin.totalCedulas(UserId());
                models.avisos = await vAvisos.GetAvisosCASESGByUsuario(UserId());
                models.avisosU = await vAvisos.GetAvisosVisibles(UserId());
                return View(models);
            }
            else
            {
                return Redirect("/error/perfilNotFound");
            }
        }

        [HttpGet]
        [Route("/home/download/casesg2")]
        public IActionResult getPlantilla()
        {
            string fileName = @"e:\Plantillas CASESGV2\DocsV2\Guia\Guia rápida del sistema CASESG_2.0.pdf";
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, "application/pdf", "Guia_CASESGV2.pdf");
        }

        [Authorize]
        [Route("/home/listado/{estatus?}")]
        public async Task<IActionResult> Concentrado(string estatus)
        {
            List<VCedulas> pre = await vRepositorioLogin.ConcentradoCedulas(UserId(),estatus);
            return View(pre);
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Home");
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<ActionResult<DatosUsuario>> Login(string username, string password,string returnUrl)
        {
            //Validamos a traves del Web Service los datos de Login
            var client = new ServicioSeguridadClient();
            ValidaUsuarioPorSistemaCompletoResponse validacion = await client.ValidaUsuarioPorSistemaCompletoAsync(277, username, password, "");
            TServicioValidacion validacionRespuesta = validacion.ValidaUsuarioPorSistemaCompletoResult;

            DatosUsuario dtUser = new DatosUsuario();
            if (validacionRespuesta.EsValido)
            {
                int existUser = await vRepositorioLogin.buscaUsuario(validacionRespuesta.DatosUsuario.Nodes[1].ToString(), username, password);
                if (existUser > 0)
                {

                    List<VModulosUsuario> modulos = null;
                    dtUser = await vRepositorioLogin.login(username, password);
                    var claims = new List<Claim>();
                    
                    modulos = await vRepositorioLogin.getModulosByUser((int)dtUser.Id);

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, dtUser.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, dtUser.Empleado));
                    claims.Add(new Claim(ClaimTypes.Role, dtUser.Perfiles));//contiene si es o no admin
                    claims.Add(new Claim(ClaimTypes.SerialNumber, dtUser.Expediente.ToString()));
                    claims.Add(new Claim(ClaimTypes.Sid, dtUser.ClaveInmueble.ToString()));
                    claims.Add(new Claim("Modulos", JsonConvert.SerializeObject(modulos)));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    return Redirect(returnUrl != null ? returnUrl : "/Home");
                }
                else
                {
                    int insertUser = await vRepositorioUsuarios.insertaUsuario(validacionRespuesta.DatosUsuario.Nodes[1].ToString(), password);
                    if (insertUser != 0)
                    {
                        List<VModulosUsuario> modulos = null;
                        dtUser = await vRepositorioLogin.login(username, password);
                        var claims = new List<Claim>();
                        modulos = await vRepositorioLogin.getModulosByUser((int)dtUser.Id);
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, dtUser.Id.ToString()));
                        claims.Add(new Claim(ClaimTypes.Name, dtUser.Empleado));
                        claims.Add(new Claim(ClaimTypes.Role, dtUser.Perfiles));//contiene si es o no admin
                        claims.Add(new Claim(ClaimTypes.SerialNumber, dtUser.Expediente.ToString()));
                        claims.Add(new Claim(ClaimTypes.Sid, dtUser.ClaveInmueble.ToString()));
                        claims.Add(new Claim("Modulos", JsonConvert.SerializeObject(modulos)));
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        return Redirect(returnUrl != null ? returnUrl : "/Home");
                    }
                }
            }
            TempData["Error"] = validacionRespuesta.MensajeError;
            return View("login");
        }

        private string isAdmin(List<VModulosUsuario> modulos)
        {
            foreach (var mm in modulos)
            {
                if (mm.PerfilId == 1)
                {
                    return 1 + "";
                }
            }
            return "";
        }

        [Authorize]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("/Account/AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Route("/CedulasEstatus/{estatus}")]
        public async Task<IActionResult> CedulasEstatus(string estatus)
        {
            if (!User.Claims.ElementAt(5).Value.Equals("[]"))
            {
                List<Dashboard> ds = await vRepositorioLogin.CedulasEstatus(UserId(), estatus);
                return View(ds);
            }
            return Redirect("/error/perfilNotFound");
        }

        [HttpGet]
        [Route("/getEstatus")]
        public async Task<IActionResult> CedulasEstatus()
        {
            if (!User.Claims.ElementAt(5).Value.Equals("[]"))
            {
                List<Dashboard> ds = await vRepositorioLogin.totalCedulas(UserId());
                return Ok(ds);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

    }
}
