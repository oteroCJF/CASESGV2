using CedulasEvaluacion.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class Oficio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int Anio { get; set; }
        public int NumeroOficio { get; set; }
        public string Servicio { get; set; }
        public string Estatus { get; set; }
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public decimal SubtotalOficio { get; set; }
        public decimal TotalOficio { get; set; }
        public decimal ImporteOficio { get; set; }
        public decimal ImportePenas { get; set; }
        public DateTime FechaTramitado { get; set; }
        public DateTime FechaPagado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public List<DetalleCedula> cedulasOficio { get; set; }
        public List<DetalleCedula> facturas { get; set; }
        public List<DetalleCedula> detalleCedulas{ get; set; }

        /*Parametros para Filtros*/
        public int InmuebleId { get; set; }
        public string Mes { get; set; }

        /*Datos Finales*/
        public decimal ImporteFacturado { get; set; }
        public decimal ImporteNC { get; set; }
    }
}
