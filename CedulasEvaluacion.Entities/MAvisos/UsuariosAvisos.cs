using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MAvisos
{
    public partial class UsuariosAvisos
    {
        public int UsuarioId { get; set; }
        public bool Visible { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
