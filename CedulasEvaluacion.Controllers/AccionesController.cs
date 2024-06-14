using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class AccionesController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IHostingEnvironment environment;

        public AccionesController(IRepositorioEvaluacionServicios iCedula, IRepositorioEntregablesCedula eiEntregables, IRepositorioUsuarios iUsuarios,
                                    IRepositorioInmuebles iVInmueble, IHostingEnvironment environment)
        {
            this.vCedula = iCedula ?? throw new ArgumentNullException(nameof(iCedula));
            this.vUsuarios= iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vEntregables = eiEntregables ?? throw new ArgumentNullException(nameof(eiEntregables));
            this.environment = environment;
        }

        [HttpGet]
        [Route("/view/entregable/{folio?}/{nombre?}")]
        public IActionResult verProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/view/entregable/{folio?}/{guia?}/{archivo}")]
        public IActionResult verReferenciasActas(string tipo, string folio, string guia, string archivo)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Seguimientos Estafeta\\" + folio + "\\"+guia;
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/view/actadeRobo/{folio?}/{nombre?}")]
        public IActionResult verActadeRobo(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\Actas de Robo";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/view/actaExtravio/{folio?}/{nombre?}")]
        public IActionResult verActaExtravio(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\Actas de Extravío";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/view/acuseFinancieros/{numero}/{acuse}")]
        public IActionResult verAcuseFinancieros(string numero,string acuse)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Oficios Financieros\\" + numero+ "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, acuse);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }
        
        [HttpGet]
        [Route("/view/servicioBasico/{servicio}/{mes}/{archivo}")]
        public IActionResult verServicioBasico(string servicio,string mes,string archivo)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Facturas SB\\" + servicio + "\\"+mes+"\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, archivo);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        /*Flujo para los estatus*/
        [HttpGet]
        [Route("/entregables/flujo/cae/{cedula?}/{estatus?}")]
        public async Task<IActionResult> GetFlujoCedulaCAE(int cedula, string estatus)
        {
            int exists = 0;
            exists = await vEntregables.GetFlujoCedulaCAE(cedula, estatus);
            if (exists != -3)
            {
                return Ok(exists);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/entregables/flujo/car/{cedula?}/{estatus?}")]
        public async Task<IActionResult> GetFlujoCedulaCAR(int cedula, string estatus)
        {
            int exists = 0;
            exists = await vEntregables.GetFlujoCedulaCAR(cedula, estatus);
            if (exists != -3)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/entregables/validaCedula/{cedula?}")]
        public async Task<IActionResult> validaCedulaDAS(int cedula)
        {
            int valida = 0;
            valida = await vEntregables.validaCedulaDAS(cedula);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }
        
        [HttpPost]
        [Route("/cedula/solicitudNC")]
        public async Task<IActionResult> SolicitudNC([FromBody]CedulaEvaluacion cedula)
        {
            int valida = await vCedula.SolicitudNC(cedula);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/cedula/notificacionCedula/{id}")]
        public async Task<IActionResult> EnviaCorreoCedula(int id)
        {
            CedulaEvaluacion cedula = await vCedula.CedulaById(id);
            cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
            List<DestinatariosCorreo> usuarios = await vUsuarios.getDestinatariosCorreo(cedula.ServicioId);
            var plantilla = await vCedula.getPlantillaCorreo(id);
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("casesg_das@cjf.gob.mx", "CASESG DAS", Encoding.UTF8);//Correo de salida
            correo.To.Add(cedula.usuarios.correo_electronico); //Correo destino?
            //correo.To.Add("jymiranda@cjf.gob.mx"); //Correo destino?
            foreach (var em in usuarios)
            {
                correo.CC.Add(em.CorreoElectronico); //Correo destino?
            }
            correo.Subject = "CASESG: Folio: "+cedula.Folio+" - Servicio: "+usuarios[0].Servicio+" - Mes: "+cedula.Mes+" - Estatus: "+cedula.Estatus; //Asunto
            correo.Body = plantilla; //Mensaje del correo
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Host = "correo.cjf.gob.mx"; //Host del servidor de correo
            smtp.Port = 25; //Puerto de salida
            smtp.Credentials = new System.Net.NetworkCredential("casesg_das@cjf.gob.mx", "Ajusco.2023");//Cuenta de correo
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { 
                return true; 
            };
            smtp.EnableSsl = true;//True si el servidor de correo permite ssl
            smtp.Send(correo);
            return Ok();
        }

        [HttpGet]
        [Route("/cedula/notificacionNC/{id}")]
        public async Task<IActionResult> EnviaCorreoSolicitudNC(int id)
        {
            try
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                List<DestinatariosCorreo> usuarios = await vUsuarios.getDestinatariosCorreo(cedula.ServicioId);
                var plantilla = "";
                if (cedula.Calificacion == Convert.ToDecimal(10.0))
                {
                    plantilla = await vCedula.getPlantillaCorreoSNCSD(id);
                }
                else
                {
                    plantilla = await vCedula.getPlantillaCorreoSNC(id);
                }
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("casesg_das@cjf.gob.mx", "CASESG DAS", Encoding.UTF8);//Correo de salida
                correo.To.Add(cedula.usuarios.correo_electronico); //Correo destino?
                //correo.To.Add("jymiranda@cjf.gob.mx"); //Correo destino?
                if (cedula.ServicioId == 12)
                {
                    correo.CC.Add("adriana.suarez@ck.com.mx");
                    correo.CC.Add("diana.sandoval@ck.com.mx"); 
                }
                foreach (var em in usuarios)
                {
                    correo.CC.Add(em.CorreoElectronico); //Correo destino?
                }
                correo.Subject = "Incidencias - " + cedula.inmuebles.Nombre + " - " + usuarios[0].Servicio + " - " + cedula.Folio; //Asunto
                correo.Body = plantilla; //Mensaje del correo
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = "correo.cjf.gob.mx"; //Host del servidor de correo
                smtp.Port = 25; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential("casesg_das@cjf.gob.mx", "Ajusco.2023");//Cuenta de correo
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                    return true;
                };
                smtp.EnableSsl = true;//True si el servidor de correo permite ssl
                smtp.Send(correo);
                return Ok();
            }catch(Exception ex)
            {
                string msg = ex.Message;
                return BadRequest();
            }
        }

        /*Flujo para los estatus*/
    }
}
