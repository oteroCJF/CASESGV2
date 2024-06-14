using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class Rubros
    {
        public int ServicioId { get; set; }
        public string Tipo { get; set; }
        public string Abreviacion { get; set; }
        public string Valor { get; set; }
    }
}
