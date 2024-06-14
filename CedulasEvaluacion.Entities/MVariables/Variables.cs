using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MVariables
{
    public partial class Variables
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int NoPregunta { get; set; }
        public string Tipo { get; set; }
        public string Abreviacion { get; set; }
        public string Valor { get; set; }
        public bool Cierre { get; set; }
        public bool NoAplica { get; set; }
        public bool NoEntrego { get; set; }
        public bool NoRealizo { get; set; }
        public bool Incidencias { get; set; }
        public bool SUA { get; set; }
        public bool Numero { get; set; }
        public bool Fecha { get; set; }
        public bool Fechas { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
