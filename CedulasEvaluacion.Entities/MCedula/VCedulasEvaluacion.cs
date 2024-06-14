using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class VCedulasEvaluacion
    {
        public int Id { get; set; }
        public string Estatus { get; set; }
        public string Prioridad { get; set; }
        public string Icono { get; set; }
        public string Fondo { get; set; }
        public string Mes { get; set; }
        public int TotalCedulas { get; set; }

    }
}
