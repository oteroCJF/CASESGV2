using System;
using System.Collections.Generic;
using System.Text;

namespace CASESGCedulasEvaluacion.Entities.Vistas
{
    public partial class VInmueble
    {
        public int Id { get; set; }
        public string ClaveAdministracion { get; set; }
        public string Administracion { get; set; }
        public string ClaveInmueble { get; set; }
        public string Tipo { get; set; }
        public string Inmueble { get; set; }
        public string Direccion { get; set; }
    }
}
