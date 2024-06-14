using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class PeriodoEvaluacion
    {
        public int Anio { get; set; }
        public string Empresa { get; set; }
        public string Contrato { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public string MesInicial { get; set; }
        public string MesFinal { get; set; }
        public string Convenios { get; set; }
        public string Servicio { get; set; }
        public decimal Calificacion { get; set; }
    }
}
