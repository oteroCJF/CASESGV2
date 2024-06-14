using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MContratos;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class ModelosCR
    {
        public int ServicioId { get; set; }
        public List<ContratosServicio> contratos { get; set; } = new List<ContratosServicio>();
        public List<CatalogoServicios> servicios { get; set; } = new List<CatalogoServicios>();
        public List<Variables> anios { get; set; } = new List<Variables>();
        public List<Variables> rubros { get; set; } = new List<Variables>();
        public List<Meses> meses { get; set; } = new List<Meses>();
    }
}
