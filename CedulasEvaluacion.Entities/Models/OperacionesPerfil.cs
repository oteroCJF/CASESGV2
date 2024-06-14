using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class OperacionesPerfil
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public int OperacionId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public virtual Operaciones Operacion { get; set; }
        public virtual Perfiles Perfil { get; set; }
    }
}
