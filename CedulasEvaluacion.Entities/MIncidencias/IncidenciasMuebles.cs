using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasMuebles
    {
        public int Id { get; set; }
        public int CedulaMuebleId { get; set; }
        public int DiasAtraso { get; set; }
        public string Tipo { get; set; }
        public string Pregunta { get; set; }
        public DateTime FechaSolicitud{ get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string Comentarios { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
