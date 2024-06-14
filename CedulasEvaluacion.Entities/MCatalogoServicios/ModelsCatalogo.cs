using CedulasEvaluacion.Entities.MContratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Entities.MCatalogoServicios
{
    public partial class ModelsCatalogo
    {
        public CatalogoServicios servicio { get; set; }
        public ContratosServicio contrato { get; set; }
        public List<ContratosServicio> contratos  { get; set; }
        public List<EntregablesContrato> entregables { get; set; }
    }
}
