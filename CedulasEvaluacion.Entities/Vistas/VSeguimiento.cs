using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VSeguimiento
    {
        public int Id { get; set; }
        public int CedulaId { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Folio { get; set; }
        public string Inmueble { get; set; }
        public string EstatusCedula { get; set; }
        public string EstatusSeguimiento { get; set; }
        public string Usuario { get; set; }
        public int TotalIncidencias { get; set; }

        public List<IncidenciasMensajeria> incidencias { get; set; } = new List<IncidenciasMensajeria>();
        public List<Entregables> entregables { get; set; } = new List<Entregables>();

    }
}
