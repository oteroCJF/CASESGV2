using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Modulos
    {
        public Modulos()
        {
            Operaciones = new HashSet<Operaciones>();
        }

        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public virtual ICollection<Operaciones> Operaciones { get; set; }
    }
}
