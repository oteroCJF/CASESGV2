using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class CedulasOficio
    {
        public int OficioId { get; set; }
        public int FacturaId { get; set; }
        public int CedulaId { get; set; }
        public int ServicioId { get; set; }
    }
}
