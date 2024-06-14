using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MLecturasAgua
{
    public partial class DashboardLectura
    {
        public string Inmueble { get; set; }
        public string Fondo { get; set; }
        public string FondoHexadecimal { get; set; }
        public int TotalLecturas { get; set; }
        public int InmuebleId { get; set; }
    }
}
