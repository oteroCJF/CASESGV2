using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Operaciones
    {
        public Operaciones()
        {
            OperacionesPerfil = new HashSet<OperacionesPerfil>();
        }

        public int Id { get; set; }
        public int ModuloId { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public virtual Modulos Modulo { get; set; }
        public virtual ICollection<OperacionesPerfil> OperacionesPerfil { get; set; }
    }
}
