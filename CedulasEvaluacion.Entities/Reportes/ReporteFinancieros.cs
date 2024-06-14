using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Reportes
{
    public class ReporteFinancieros
    {
        public int Id { get; set; }
        public string Inmueble { get; set; }
        public string Folio { get; set; }
        public string FolioFactura { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public int Inasistencias { get; set; }
        public decimal Calificacion { get; set; }
        public decimal PenaPO { get; set; }
        public decimal PenaUniforme { get; set; }
        public decimal PenaEquipo { get; set; }
        public decimal PenaCapacitacion { get; set; }
        public decimal PenaCalificacion { get; set; }
        public decimal TotalInmueble { get; set; }
        public string Estatus { get; set; }
    }
}
