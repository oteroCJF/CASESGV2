using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class ModelsFinancieros
    {
        public List<DashboardFinancieros> dashboard { get; set; }
        public List<Oficio> oficio { get; set; }
        public List<Variables> listaEntregables { get; set; }
        public List<Inmueble> inmuebles { get; set; }
        public int Anio { get; set; }
        public string Servicio { get; set; }
        public int ServicioId { get; set; }
        public string Descripcion { get; set; }
    }
}
