using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VModulosUsuario
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public int ServicioId { get; set; }
        public int ModuloId { get; set; }
        public string Empleado { get; set; }
        public string Servicio { get; set; }
        public string Modulo { get; set; }
        public string URL { get; set; }
        public string Icono { get; set; }
        public bool ServicioBasico { get; set; }
    }
}
