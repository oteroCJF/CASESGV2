using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class Penalizaciones
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int Pregunta { get; set; }
        public int PreguntaReal { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion{ get; set; }
        public string Abreviacion { get; set; }
        public string Icono { get; set; }
        public string Valor{ get; set; }
        public string TipoDeduccion { get; set; }
    }
}
