using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MContratos
{
    public class ConveniosContrato
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public string NoContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoMin { get; set; }
        public decimal MontoMax { get; set; }
        public int VolumetriaMin { get; set; }
        public int VolumetriaMax { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
