using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MAvisos
{
    public partial class AvisosPerfil
    {
        public int AvisoId { get; set; }
        public int PerfilId { get; set; }
        public Perfiles perfil { get; set; }
    }
}
