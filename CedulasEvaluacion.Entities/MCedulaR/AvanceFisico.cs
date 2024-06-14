using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class AvanceFisico
    {
        public long ServiciosContratados { get; set; }
        public long ServiciosDevengados { get; set; }
        public long ServiciosPorDevengar { get; set; }
        public decimal PorcentajeFisico { get; set; }
    }
}
