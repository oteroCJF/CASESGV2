using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class DestinatariosCorreo
    {
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string CorreoElectronico { get; set; }
        public string Servicio { get; set; }
    }
}
