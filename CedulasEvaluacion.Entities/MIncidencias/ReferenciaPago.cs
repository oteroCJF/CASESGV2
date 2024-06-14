using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class ReferenciaPago
    {
        public int Id { get; set; }
        public int IncidenciaId { get; set; }
        public string NumeroReferencia { get; set; }
        public string Contrato { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string ArchivoReferencia { get; set; }
        public IFormFile AReferencia { get; set; }
        public IFormFile APago { get; set; }
        public string ReportePago { get; set; }
        public string Estatus { get; set; }
        public string NumeroGuia { get; set; }
        public string FolioCedula { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
