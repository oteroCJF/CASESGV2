using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class HistorialEntregables
    {
        public string Servicio { get; set; }
        public int ServicioId { get; set; }
        public string Tipo { get; set; }
        public int CedulaId { get; set; }
        public int UsuarioId { get; set; }
        public string Estatus { get; set; }
        public string Comentarios { get; set; }
        public DateTime? FechaCreacion { get; set; }

        public virtual Usuarios usuarios { get; set; }
    }
}
