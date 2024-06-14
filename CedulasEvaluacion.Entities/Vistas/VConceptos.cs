using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VConceptos
    {
        public decimal Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string ObservacionGeneral { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA{ get; set; }
    }
}
