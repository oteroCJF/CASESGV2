using CedulasEvaluacion.Entities.MCatalogoServicios;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFirmantes
{
    public class ModelsFirmantes
    {
        public List<FirmantesServicio> firmantes { get; set; }
        public List<CatalogoServicios> catalogo { get; set; }
    }
}
