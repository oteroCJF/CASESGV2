using CedulasEvaluacion.Entities.MCatalogoServicios;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MContratos
{
    public partial class EntregablesContrato
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string TipoContrato { get; set; }
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string Estatus { get; set; }
        public decimal MontoGarantia { get; set; }
        public decimal MontoPenalizacion { get; set; }
        public long Tamanio { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime InicioPeriodo { get; set; }
        public DateTime FinPeriodo { get; set; }
        public int DiasAtraso { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }

        public ContratosServicio contrato { get; set; }
        public CatalogoServicios servicio{ get; set; }
    }
}
