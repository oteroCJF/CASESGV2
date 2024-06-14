using CedulasEvaluacion.Entities.MFacturas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class ActaEntregaRecepcion
    {
        public int Id { get; set; }
        public string Mes { get; set; }
        public string Folio { get; set; }
        public int Anio { get; set; }
        public string Servicio { get; set; }
        public string TipoServicio { get; set; }
        public string EncabezadoInmueble { get; set; }
        public string InmuebleEvaluado { get; set; }
        public string TipoInmueble { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public string Administrador { get; set; }
        public string PuestoAutoriza { get; set; }
        public string Reviso { get; set; }
        public string Elaboro { get; set; }
        public string Prestador { get; set; }
        public string Folios { get; set; }
        public string FechasTimbrado { get; set; }
        public string Cantidad { get; set; }
        public string Total { get; set; }
        public string FoliosNC { get; set; }
        public string TotalServicios { get; set; }
        public string FechasTimbradoNC { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal{ get; set; }
        public string CantidadNC { get; set; }
        public string TotalNC { get; set; }
        public string ConceptosComedor { get; set; }

    }
}
