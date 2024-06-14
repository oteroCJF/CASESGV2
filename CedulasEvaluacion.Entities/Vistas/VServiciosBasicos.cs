using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
   public partial class VServiciosBasicos
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int InmuebleId { get; set; }
        public int Anio { get; set; }
        public string Servicio { get; set; }
        public string Abreviacion { get; set; }
        public string Inmueble { get; set; }
        public string Mes { get; set; }
        public string Estatus { get; set; }
        public string Fondo { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public decimal Monto { get; set; }
        public string FondoHexadecimal { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
