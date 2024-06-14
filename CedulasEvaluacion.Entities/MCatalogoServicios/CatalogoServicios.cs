using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCatalogoServicios
{
    public partial class CatalogoServicios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Abreviacion { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string Fondo { get; set; }
        public bool ServicioBasico { get; set; }
    }
}
