using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class RespuestasEncuesta
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public bool Respuesta { get; set; }
        public string Detalles { get; set; }
        public bool? Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
    }
}
