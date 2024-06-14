using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasResiduos
    {
        public int Id { get; set; }
        public int Pregunta { get; set; }
        public int CedulaResiduosId { get; set; }
        public string Tipo { get; set; }
        public string Comentarios { get; set; }
    }
}
