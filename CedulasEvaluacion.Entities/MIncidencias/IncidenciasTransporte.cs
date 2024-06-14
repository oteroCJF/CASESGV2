using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasTransporte
    {
        public int Id { get; set; }
        public int CedulaTransporteId { get; set; }
        public string Tipo { get; set; }
        public string Pregunta { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public TimeSpan HoraPresentada { get; set; }
        public string Comentarios { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
