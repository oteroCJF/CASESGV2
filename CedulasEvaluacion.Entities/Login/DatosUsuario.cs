using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Login
{
    public partial class DatosUsuario
    {
        public int? Id { get; set; }
        public string Perfil  { get; set; }
        public string ModuloProveniente { get; set; }
        public string Descripcion { get; set; }
        public int Expediente { get; set; }
        public string Usuario { get; set; }
        public string Empleado { get; set; }
        public string CorreoElectronico { get; set; }
        public string Puesto{ get; set; }
        public string Area { get; set; }
        public int ClaveInmueble { get; set; }
        public string Estatus{ get; set; }
        public string Perfiles { get; set; }



    }
}
