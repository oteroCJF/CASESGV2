using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class CedulaEvaluacion
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int InmuebleId { get; set; }
        public int Maniobras { get; set; }
        public int InmuebleDestinoId { get; set; }
        public int UsuarioId { get; set; }
        public string Folio { get; set; }
        public string Icono { get; set; }
        public string Fondo { get; set; }
        public string TipoServicio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public int TotalCedulas { get; set; }
        public int TotalAlcances { get; set; }
        public decimal? Calificacion { get; set; }
        public decimal? PenaCalificacion { get; set; }
        public decimal TotalBajoDemanda { get; set; }
        public string Estatus { get; set; }
        public string URL { get; set; }
        public bool Alcance { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public decimal TotalMontoFactura { get; set; }
        public decimal TotalDeductivas { get; set; }
        public decimal TotalPenalizaciones { get; set; }

        public string divInmueble { get; set; }
        public string divDeductivas { get; set; }
        public string Comentarios { get; set; }

        public virtual List<InmueblesUsuarios> listI { get; set; }
        public virtual Inmueble inmuebles { get; set; }
        public virtual Inmueble inmuebleDestino { get; set; }
        public virtual Usuarios usuarios { get; set; }
        public virtual List<Facturas> facturas { get; set; }
        public virtual List<Entregables> iEntregables { get; set; }
        public virtual List<Entregables> iAlcances { get; set; }
        public virtual List<Preguntas> preguntas{ get; set; }
        public List<RespuestasEncuesta> RespuestasEncuesta { get; set; }
        public List<HistorialCedulas> historialCedulas { get; set; }
        public List<HistorialEntregables> historialEntregables { get; set; }
        public ModelsIncidencias incidencia { get; set; }
        public ModelsIncidencias incidencias { get; set; }
        public virtual Variables horarioComedor { get; set; }
        public virtual List<Variables> pestanas { get; set; }
        public virtual List<Penalizaciones> penalizaciones { get; set; }
        public virtual List<HistorialNotasCredito> historialNC { get; set; }
    }
}
