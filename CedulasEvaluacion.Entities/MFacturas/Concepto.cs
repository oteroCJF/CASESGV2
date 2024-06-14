using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class Concepto
    {
        public long FacturaId { get; set; }
        public decimal Cantidad { get; set; }
        public long ClaveProdServ { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public string ObservacionGeneral { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public DatosTotales datosTotales { get; set; }
        public DatosExtra datosExtra { get; set; }

    }
}
