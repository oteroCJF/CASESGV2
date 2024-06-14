using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class IncidenciasMensajeria
    {
        public int Id { get; set; }
        public int CedulaMensajeriaId { get; set; }
        public int Pregunta { get; set; }
        public decimal Sobrepeso { get; set; }
        public string Tipo { get; set; }
        public string NumeroGuia { get; set; }
        public long CodigoRastreo { get; set; }
        public string TipoServicio { get; set; }
        public string Archivo { get; set; }
        public string Folio { get; set; }
        public bool Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public int TotalIncidencias { get; set; }
        public int TotalAcuses { get; set; }
        public string Acuse { get; set; }
        public string NombreActa { get; set; }
        public IFormFile Excel { get; set; }
        public IFormFile ActaRobo { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime FechaEfectiva { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public List<ReferenciaPago> referencias { get; set; } = new List<ReferenciaPago>();

    }
}
