using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MAlcances
{
    public partial class MAlcances
    {
        public List<CedulaEvaluacion> concentrado { get; set; }
        public List<CatalogoServicios> catalogo { get; set; }
        public List<Variables> anios { get; set; }
        public int Anio { get; set; }
        public int ServicioId { get; set; }
    }
}
