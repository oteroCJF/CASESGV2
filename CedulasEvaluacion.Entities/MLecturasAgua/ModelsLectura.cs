using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MLecturasAgua
{
    public partial class ModelsLectura
    {
        public int Anio { get; set; }
        public List<DashboardLectura> dashboard { get; set; }
        public List<LecturaAgua> lectura { get; set; }
        public Inmueble inmueble { get; set; }
    }
}
