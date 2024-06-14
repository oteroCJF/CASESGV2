using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CedulasEvaluacion.Entities.Models;
using System.Threading.Tasks;
using System.Drawing;
using CedulasEvaluacion.Entities.Vistas;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Linq;
using System.Globalization;
using CedulasEvaluacion.Entities.MFacturas;
using Microsoft.AspNetCore.Authorization;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IRepositorioEvaluacionServicios vCedula;

        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioDocuments vDocuments;
        private readonly IRepositorioEntregablesCedula vEntregables;

        private readonly IRepositorioIncidenciasMensajeria iMensajeria;
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;
        private readonly IRepositorioIncidenciasComedor iComedor;
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IRepositorioIncidenciasConvencional iConvencional;

        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private string[] tiposCelular = {"","Alta_Equipo", "Alta", "Baja", "Reactivacion", "Suspension", "Perfil", "SIM", "CambioNumeroRegion",
                                          "VozDatos", "Diagnostico", "Reparacion"};
        private string[] tiposConv = { "", "contratacion_instalacion", "cableado", "entregaAparato", "cambioDomicilio", "reubicacion", "identificadorLlamadas", "troncales", "internet",
                                          "serviciosTelefonia", "cancelacion", "reportesFallas"};

        public DocumentsController(IRepositorioEvaluacionServicios viCedula, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasFumigacion iIncidenciasFumigacion, IRepositorioIncidenciasAgua iIncidenciasAgua, IRepositorioIncidenciasResiduos iiResiduos,
                                    IRepositorioIncidenciasTransporte iiTransporte, IRepositorioIncidenciasTraslado iiTraslado,
                                    IRepositorioPerfiles iRepositorioPerfiles,IRepositorioFacturas iFacturas, IRepositorioIncidenciasMensajeria iiMensajeria,
                                    IRepositorioIncidenciasCelular ivCelular, IRepositorioIncidenciasConvencional ivConvencional,
                                    IRepositorioDocuments ivDocuments, IRepositorioIncidenciasMuebles iiMuebles,
                                    IRepositorioIncidenciasAnalisis iiAnalisis, IRepositorioInmuebles viInmuebles,
                                    IRepositorioFacturas viFacturas, IRepositorioIncidencias iiIncidencias, IRepositorioIncidenciasComedor iiComedor)
        {

            this.vIncidencias = iiIncidencias ?? throw new ArgumentNullException(nameof(iiIncidencias));
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vDocuments = ivDocuments ?? throw new ArgumentNullException(nameof(ivDocuments));

            this.iCelular = ivCelular ?? throw new ArgumentNullException(nameof(ivCelular));
            this.iMensajeria = iiMensajeria ?? throw new ArgumentNullException(nameof(iiMensajeria));

            this.iResiduos = iiResiduos ?? throw new ArgumentNullException(nameof(iiResiduos));

            this.iTransporte = iiTransporte ?? throw new ArgumentNullException(nameof(iiTransporte));

            this.iTraslado = iiTraslado ?? throw new ArgumentNullException(nameof(iiTraslado));

            this.iAgua = iIncidenciasAgua ?? throw new ArgumentNullException(nameof(iIncidenciasAgua));
            this.iAnalisis = iiAnalisis ?? throw new ArgumentNullException(nameof(iiAnalisis));

            this.iFumigacion = iIncidenciasFumigacion ?? throw new ArgumentNullException(nameof(iIncidenciasFumigacion));

            this.iMuebles = iiMuebles ?? throw new ArgumentNullException(nameof(iiMuebles));

            this.iConvencional = ivConvencional ?? throw new ArgumentNullException(nameof(ivConvencional));
            this.iComedor = iiComedor ?? throw new ArgumentNullException(nameof(iiComedor));

            this.vInmuebles = viInmuebles ?? throw new ArgumentNullException(nameof(viInmuebles));

            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.vFacturas = viFacturas ?? throw new ArgumentNullException(nameof(viFacturas));
        }


        [Route("/documents/actaLimpieza/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionLimpieza(int id,int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);
            
            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|",acta.EncabezadoInmueble, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await vIncidencias.getIncidencias(id)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|","\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Limpieza_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaFumigacion/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionFumigacion(int id,int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Fumigacion.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.EncabezadoInmueble, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);


            if ((await iFumigacion.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Fumigacion_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaMensajeria/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionMensajeria(int id,int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Mensajeria.docx";
            document.LoadFromFile(path);

            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
                document.Replace("|Coordinacion|", "COORDINACIÓN DE CONTROL OPERATIVO DE ADMINISTRACIONES DE EDIFICIOS", false, true);
                document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Coordinacion|", "COORDINACIÓN DE ADMINISTRACIÓN REGIONAL", false, true);
                document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.TipoInmueble) + " " + CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.Estado), false, true);
            }

            document.Replace("|AdministracionInmueble|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iMensajeria.getIncidenciasByTipoMensajeria(id, "Recoleccion")).Count == 0 && (await iMensajeria.getIncidenciasByTipoMensajeria(id, "Entrega")).Count == 0 &&
                (await iMensajeria.getIncidenciasByTipoMensajeria(id, "Acuses")).Count == 0 && (await iMensajeria.getIncidenciasByTipoMensajeria(id, "Mal Estado")).Count == 0 &&
                (await iMensajeria.getIncidenciasByTipoMensajeria(id, "Extraviadas")).Count == 0 && (await iMensajeria.getIncidenciasByTipoMensajeria(id, "Robadas")).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);


            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Mensajeria_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaAgua/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionAgua(int id, int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Agua.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.EncabezadoInmueble, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Agua_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaRPBI/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionRPBI(int id,int servicio)
        {
            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 RPBI.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.EncabezadoInmueble, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Medico|", acta.Elaboro, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);

            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_RPBI_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaAnalisis/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionAnalisis(int id, int servicio)
        {
            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Analisis.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.EncabezadoInmueble, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|ResponsableCENDI|", acta.Elaboro, false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Analisis_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaConvencional/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionTelConvencional(int id, int servicio)
        {
            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);
            List<IncidenciasConvencional> convencional = new List<IncidenciasConvencional>();
            var totalIncidencias = 0;

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción Telefonía Convencional.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

           
            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", Convert.ToDateTime(cedula.FechaCreacion).Day + "", false, true);
            document.Replace("|Mes|", Convert.ToDateTime(cedula.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", Convert.ToDateTime(cedula.FechaCreacion).Year + "", false, true);
            document.Replace("|Hora|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            document.Replace("|DiaActual|", Convert.ToDateTime(cedula.FechaCreacion).Day + " de " + Convert.ToDateTime(cedula.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + Convert.ToDateTime(cedula.FechaCreacion).Year, false, true);
            document.Replace("|HoraActual|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            document.Replace("|Hora|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            for (var i = 0; i < tiposCelular.Length; i++)
            {
                convencional = await iConvencional.ListIncidenciasTipoConvencional(id, tiposConv[i]);
                for (var j = 0; j < convencional.Count; j++)
                {
                    if (convencional[j].HorasRetraso != 0 || convencional[j].DiasRetraso != 0)
                    {
                        totalIncidencias = 1;
                        break;
                    }
                }
                if (totalIncidencias != 0)
                {
                    break;
                }
            }

            if (totalIncidencias == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Analisis_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaCelular/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionTelCelular(int id, int servicio)
        {
            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);
            List<IncidenciasCelular> celular = new List<IncidenciasCelular>();
            int totalIncidencias = 0;

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción Telefonía Celular.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();


            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", Convert.ToDateTime(cedula.FechaCreacion).Day + "", false, true);
            document.Replace("|Mes|", Convert.ToDateTime(cedula.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", Convert.ToDateTime(cedula.FechaCreacion).Year + "", false, true);
            document.Replace("|Hora|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            document.Replace("|DiaActual|", Convert.ToDateTime(cedula.FechaCreacion).Day + " de " + Convert.ToDateTime(cedula.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + Convert.ToDateTime(cedula.FechaCreacion).Year, false, true);
            document.Replace("|HoraActual|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            document.Replace("|Hora|", Convert.ToDateTime(cedula.FechaCreacion).Hour + ":00", false, true);
            for (var i = 0; i<tiposCelular.Length;i++)
            {
                celular = await iCelular.ListIncidenciasTipoCelular(id, tiposCelular[i]);
                for(var j=0; j<celular.Count; j++)
                {
                    if(celular[j].HorasRetraso != 0 || celular[j].DiasRetraso != 0)
                    {
                        totalIncidencias = 1;
                        break;
                    }
                }
                if (totalIncidencias != 0)
                {
                    break;
                }
            }

            if (totalIncidencias == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando retrasos en ciertos requerimientos, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Celular_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaTransporte/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionTransporte(int id, int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Transporte.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.InmuebleEvaluado, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.CantidadNC.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Transporte_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaComedor/{id?}/{servicio?}")]
        public async Task<IActionResult> getActaEntregaRecepcionComedor(int id, int servicio)
        {

            ActaEntregaRecepcion acta = new ActaEntregaRecepcion();
            CedulaEvaluacion cedula = new CedulaEvaluacion();
            cedula = await vCedula.CedulaById(id);
            acta = await vDocuments.getDatosActa(id, servicio);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Comedor.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Ciudad|", acta.Estado, false, true);
            if (acta.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + acta.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + acta.Estado, false, true);
                document.Replace("|Estado|", "en " + acta.Estado, false, true);
            }

            document.Replace("|EncabezadoInmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.EncabezadoInmueble), false, true);
            document.Replace("|AdministracionInmueble|", acta.InmuebleEvaluado, false, true);
            document.Replace("|InmuebleEvaluado|", acta.InmuebleEvaluado, false, true);
            document.Replace("|Puesto|", acta.PuestoAutoriza, false, true);
            document.Replace("|PuestoFirma|", CultureInfo.CurrentCulture.TextInfo.ToUpper(acta.PuestoAutoriza), false, true);
            document.Replace("|DomicilioInmueble|", acta.Direccion, false, true);
            document.Replace("|Administrador|", acta.Administrador, false, true);
            document.Replace("|Reviso|", acta.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acta.Elaboro.ToLower()), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);
            document.Replace("|TipoServicio|", cedula.TipoServicio, false, true);
            
            document.Replace("|PeriodoEval|", (
                    cedula.FechaInicial.ToString("dd")+" de " +
                    cedula.FechaInicial.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " del " + cedula.FechaInicial.ToString("yyyy")+ " al "+
                    cedula.FechaFinal.ToString("dd") + " de "+ 
                    cedula.FechaFinal.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " del "+ cedula.FechaFinal.ToString("yyyy")
                ), false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            var incidencias = (await iComedor.GetIncidencias(id)).Count;

            if (incidencias == 0 || cedula.FechaFinal < Convert.ToDateTime("2022-11-18"))
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            document.Replace("|ImporteIVA|", acta.Total.Replace("|", "\n"), false, true);
            document.Replace("|TotalServicios|", acta.TotalServicios+" Servicio(s)", false, true);
            document.Replace("|CantidadServicios|", acta.Cantidad.Replace("|", "\n"), false, true);
            document.Replace("|FechaTimbrado|", acta.FechasTimbrado.Replace("|", "\n"), false, true);
            document.Replace("|FolioFactura|", acta.Folios.Replace("|", "\n"), false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", acta.TotalNC.Replace("|", "\n"), false, true);
            document.Replace("|CantidadNota|", acta.ConceptosComedor.Replace("|", "\n"), false, true);
            document.Replace("|TimbradoNota|", acta.FechasTimbradoNC.Replace("|", "\n"), false, true);
            document.Replace("|FolioNota|", acta.FoliosNC.Replace("|", "\n"), false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER Comedor_" + cedula.Mes + ".docx");
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }
        private string moduloLimpieza()
        {
            return "Limpieza";
        }
        private string moduloMensajeria()
        {
            return "Mensajería";
        }
        private string moduloTransporte()
        {
            return "Transporte_de_Personal";
        }
        private string moduloTraslado()
        {
            return "Traslado_de_Expedientes";
        }
        private string moduloMuebles()
        {
            return "Bienes_Muebles";
        }
        private string moduloAnalisis()
        {
            return "Análisis_Microbiológicos";
        }
        private string moduloRPBI()
        {
            return "RPBI";
        }
        private string moduloFumigacion()
        {
            return "Fumigación";
        }
        private string moduloAgua()
        {
            return "Agua_Para_Beber";
        }
        private string moduloTelefoniaCelular()
        {
            return "Telefonia_Celular";
        }
        private string moduloTelefoniaConvencional()
        {
            return "Telefonia_Convencional";
        }

    }
}
