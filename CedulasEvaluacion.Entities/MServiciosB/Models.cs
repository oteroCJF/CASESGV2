using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MServiciosB
{
    public partial class Models
    {
        public int Anio { get; set; }
        public int ServicioId { get; set; }
        public List<VServiciosBasicos> servicios { get; set; }        
        public CatalogoServicios servicio { get; set; }

    }
}
