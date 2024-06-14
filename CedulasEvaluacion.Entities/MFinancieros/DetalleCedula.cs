using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class DetalleCedula
    {
        public int Id { get; set; }
        public string Inmueble { get; set; }
        public string Folio { get; set; }
        public string Tipo { get; set; }
        public string FolioFactura { get; set; }
        public string Mes { get; set; }
        public string Serie { get; set; }
        public int Anio { get; set; }
        public int ServicioId { get; set; }
        public int FacturaId { get; set; }
        public string EstatusCedula { get; set; }
        public string EstatusFactura { get; set; }
        public string Servicio { get; set; }
        public decimal Subtotal{ get; set; }
        public decimal Total { get; set; }
        public decimal IVA { get; set; }
        public decimal Calificacion { get; set; }

        public Oficio oficio { get; set; }
    }
}
