using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public class InmueblesUsuarios
    {
        public int UsuarioId { get; set; }
        public int? Clave { get; set; }
        public string Direccion { get; set; }
        public int AdministracionId { get; set; }
        public int Tipo { get; set; }
        public string Inmueble { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }
}
