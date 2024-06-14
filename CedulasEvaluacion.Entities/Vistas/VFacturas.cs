using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VFacturas
    {
        public int Id { get; set; }
        public int CedulaId { get; set; }
        public string Tipo { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string Folio { get; set; }
        public string UUID { get; set; }
        public string Estatus { get; set; }
        public decimal IVA { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaTimbrado { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}
