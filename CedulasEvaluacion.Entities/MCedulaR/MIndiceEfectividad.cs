using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedulaR
{
    public partial class MIndiceEfectividad
    {
        public int Id { get; set; }
        public int CedulaId { get; set; }
        public int MesId { get; set; }
        public int InmuebleId { get; set; }
        public string TipoServicio { get; set; }
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public string Mes { get; set; }
        public string Inmueble { get; set; }
        public int CedulasIncidencias { get; set; }
        public int CedulasSIncidencias { get; set; }
        public int TotalIncidencias { get; set; }
        public decimal IndiceEfectividad { get; set; }
        public decimal Calificacion { get; set; }

    }
}
