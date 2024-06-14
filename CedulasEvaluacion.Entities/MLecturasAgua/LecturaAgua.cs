using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MLecturasAgua
{
    public partial class LecturaAgua
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int InmuebleId { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Cuenta { get; set; }
        public string Lectura { get; set; }
        public string Estatus { get; set; }
        public string Ubicacion { get; set; }
        public string Fondo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public List<Inmueble> inmuebles { get; set; }
        public List<EntregablesLecturasAgua> entregables { get; set; }
        public Inmueble inmueble { get; set; }

    }
}
