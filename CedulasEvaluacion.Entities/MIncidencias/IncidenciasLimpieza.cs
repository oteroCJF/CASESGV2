using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasLimpieza
    {
        public int Id { get; set; }
        public int CedulaLimpiezaId { get; set; }
        public int Pregunta { get; set; }
        public int IncidenciaId { get; set; }
        public string Tipo { get; set; }
        public string Area { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaIncidencia { get; set; }
    }
}
