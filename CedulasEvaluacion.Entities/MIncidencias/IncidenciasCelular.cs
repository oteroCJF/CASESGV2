using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasCelular
    {
        public int Id { get; set; }
        public int CedulaCelularId { get; set; }
        public int PerfilCelularId { get; set; }
        public int Pregunta { get; set; }
        public string Tipo { get; set; }
        public string Linea { get; set; }
        public int HorasAtencion { get; set; }
        public int HorasRetraso { get; set; }
        public int DiasAtencion { get; set; }
        public int DiasRetraso { get; set; }
        public string Archivo { get; set; }
        public string Folio { get; set; }
        public string Nombre { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public virtual PerfilesCelular perfilesCelular { get; set; }
        public IFormFile Excel { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaAtencion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
