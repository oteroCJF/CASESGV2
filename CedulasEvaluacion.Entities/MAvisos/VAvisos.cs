using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MAvisos
{
    public partial class VAvisos
    {
        public int Id { get; set; }
        public int AvisoId { get; set; }
        public int PerfilId { get; set; }
        public int UsuarioId { get; set; }
        public bool Activo { get; set; }
        public bool Visible { get; set; }
        public string Titulo { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
