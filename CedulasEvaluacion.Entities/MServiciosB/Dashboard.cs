using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MServiciosB
{
    public partial class Dashboard
    {
        public int Id { get; set;  }
        public string Nombre { get; set; }
        public string Abreviacion { get; set; }
        public string Fondo { get; set; }
        public string FondoHexadecimal { get; set; }
        public string Icono { get; set; }
        public int TotalFacturas { get; set; }
    }
}
