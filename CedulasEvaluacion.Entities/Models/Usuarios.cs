using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Usuarios
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public int exp { get; set; }
        public string cve_usuario { get; set; }
        public string nom_cat { get; set; }
        public string nombre_emp { get; set; }
        public string paterno_emp { get; set; }
        public string materno_emp { get; set; }
        public string cve_puesto { get; set; }
        public string nom_pue { get; set; }
        public int intentos  { get; set; }
        public DateTime? fch_vig_pswd{ get; set; }
        public string rfc_emp{ get; set; }
        public string curp_emp{ get; set; }
        public string correo_electronico { get; set; }
        public string NombreCompleto { get; set; }

        public virtual Areas areas { get; set; }
    }
}
