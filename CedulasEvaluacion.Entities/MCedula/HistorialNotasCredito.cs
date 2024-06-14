using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class HistorialNotasCredito
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int UsuarioId { get; set; }
        public int ServicioId { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoDupe { get; set; }
        public string Comentarios { get; set; }
        public bool Vencio { get; set; }
        public string Estatus { get; set; }
    }
}
