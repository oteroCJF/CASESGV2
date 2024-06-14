using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public class RespuestasAdicionales
    {
        public int Id { get; set; }
        public int CedulaLimpiezaId { get; set; }
        public string Pregunta { get; set; }
        public string Folio { get; set; }
        public int Prioridad { get; set; }
        public bool Respuesta { get; set; }
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaLimite { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion{ get; set; }

    }
}
