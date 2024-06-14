using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Entregables
    {
        public int Id { get; set; }
        public int CedulaEvaluacionId { get; set; }
        public int Anio { get; set; }
        public int InmuebleId { get; set; }
        public int ServicioBasico { get; set; }
        public int ServicioId { get; set; }
        public int EntregableId { get; set; }
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string NombreServicio { get; set; }
        public string Mes { get; set; }
        public string Estatus { get; set; }
        public string Tipo { get; set; }
        public string TipoEntregable { get; set; }
        public string Entregable { get; set; }
        public string Folio { get; set; }
        public int Tamanio { get; set; }
        public bool ValidadoDas { get; set; }
        public int DiasAtraso { get; set; }
        public string Comentarios{ get; set; }
        public string Color { get; set; }
        public string Icono { get; set; }
        public string ClienteEstafeta { get; set; }
        public bool Firmado { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public DateTime FechaPresentacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
