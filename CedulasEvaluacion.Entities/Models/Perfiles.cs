using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Perfiles
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ModuloProveniente { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public virtual List<Modulos> Modulos { get; set; }
        public virtual List<Operaciones> Operaciones { get; set; }
        
        public virtual List<OperacionesPerfil> opPerfil { get; set; }
    }
}
