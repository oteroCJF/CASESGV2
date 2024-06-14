using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class PerfilesCelular
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoPerfil { get; set; }
        public decimal CostoMensual { get; set; }
        public decimal CostoMensualIVA { get; set; }
    }
}
