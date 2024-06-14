using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MAvisos
{
    public partial class AvisosCASESG
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public List<AvisosPerfil> avisoP { get; set; }
    }
}
