using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class ArchivosCedula
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CedulaLimpiezaId { get; set; }
        public string Nombre { get; set; }
        public long Tamano { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion{ get; set; }

    }
}
