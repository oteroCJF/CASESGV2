using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasComedor
    {
        public int Id { get; set; }
        public int CedulaComedorId { get; set; }
        public int Pregunta { get; set; }
        public string Tipo { get; set; }
        public string Incumplimiento { get; set; }
        public string TipoIncidencia { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime FechaEntrega{ get; set; }
        public DateTime FechaIncidencia { get; set; }
        public string Folio { get; set; }
        public int TotalMuestras { get; set; }
        public int DiasAtraso { get; set; }
        public bool Penalizable { get; set; }
        public bool Cumplio { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
