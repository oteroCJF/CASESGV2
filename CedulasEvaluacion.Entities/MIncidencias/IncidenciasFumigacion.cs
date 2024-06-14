using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public class IncidenciasFumigacion
    {
        public int Id { get; set; }
        public int CedulaFumigacionId { get; set; }
        public int DHAtraso { get; set; }
        public string Tipo { get; set; }
        public string Pregunta { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime FechaRealizada { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public TimeSpan HoraRealizada { get; set; }
        public string Comentarios { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
