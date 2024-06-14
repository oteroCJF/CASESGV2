using CedulasEvaluacion.Entities.MAvisos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MHome
{
    public class Dashboard
    {
        public string Estatus { get; set; }
        public string Mes { get; set; }
        public string Servicio { get; set; }
        public int ServicioId { get; set; }
        public string Abreviacion { get; set; }
        public string Fondo { get; set; }
        public string Icono { get; set; }
        public int Total { get; set; }
    }
}
