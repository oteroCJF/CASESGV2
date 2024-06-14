using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class ModelsFacturas
    {
        public List<DashboardFacturas> facturasMes { get; set; }
        public List<DashboardFacturas> facturasServicio { get; set; }
        public List<DashboardFacturas> facturasParciales { get; set; }
        public List<DesgloceServicio> desgloceServicio { get; set; }
        public List<DesgloceServicio> detalle { get; set; }
        public int Anio { get; set; }
    }
}
