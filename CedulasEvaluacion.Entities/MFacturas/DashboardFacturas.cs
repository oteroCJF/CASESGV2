using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class DashboardFacturas
    {
        public int Id { get; set; }
        public int TotalFacturas { get; set; }
        public string Servicio { get; set; }
        public string Descripcion { get; set; }
        public string Estatus { get; set; }
        public string Mes { get; set; }
        public string Fondo { get; set; }
    }
}
