using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Inmueble
    {
        public int Id { get; set; }
        public int AdministracionId { get; set; }
        public int Clave { get; set; }
        public string Nombre { get; set; }
        public string Abreviacion { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public string Autoriza { get; set; }
        public int Tipo { get; set; }
        public DateTime? FechaInicioVigencia { get; set; }
        public DateTime? FechaFinVigencia { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

    }
}
