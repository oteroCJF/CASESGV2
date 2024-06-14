using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MDeductivas
{
    public partial class DesgloceDeductivas
    {
        public int Id { get; set; }
        public int Pregunta { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaIncidencia{ get; set; }
        public string Comentarios { get; set; }
        public int DiasAtraso{ get; set; }
        public decimal MontoPenalizacion{ get; set; }
    }
}
