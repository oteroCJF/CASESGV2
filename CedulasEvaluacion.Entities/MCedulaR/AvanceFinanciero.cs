using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class AvanceFinanciero
    {
        public decimal MontoContratado { get; set; }
        public decimal MontoFacturado { get; set; }
        public decimal MontoDeductivas { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoPorEjercer { get; set; }
        public decimal MontoPendientePago { get; set; }
    }
}
