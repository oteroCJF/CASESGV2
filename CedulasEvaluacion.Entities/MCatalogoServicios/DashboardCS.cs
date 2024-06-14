using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCatalogoServicios
{
    public partial class DashboardCS
    {
        public int Id { get; set; }
        public string Servicio { get; set; }
        public string Fondo { get; set; }
        public string Icono { get; set; }
        public int Total { get; set; }
    }
}
