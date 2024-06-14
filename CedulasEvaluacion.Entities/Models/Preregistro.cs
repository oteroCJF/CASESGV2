using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Preregistro
    {
        public string Nombre { get; set; }
        public string Inmueble { get; set; }
        public string Expediente { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Servicios { get; set; }
        public string Cliente { get; set; }
        public string Valida { get; set; }
        public string Revisa { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
