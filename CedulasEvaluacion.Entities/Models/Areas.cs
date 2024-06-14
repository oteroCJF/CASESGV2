using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Areas
    {

        public int Id { get; set; }

        public int ClaveInmueble { get; set; }
        public int cveArea { get; set; }
        public string cve_adscripcion { get; set; }
        public string nom_area { get; set; }
        public string nom_edo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

    }
}
