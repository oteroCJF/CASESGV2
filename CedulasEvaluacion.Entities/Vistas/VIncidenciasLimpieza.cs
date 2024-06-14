using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VIncidenciasLimpieza
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Comentarios { get; set; }
    }
}
