using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFirmantes
{
    public partial class FirmantesServicio
    {
        public int Id { get; set; }
        public int CedulaId { get; set; }
        public int UsuarioId { get; set; }
        public int InmuebleId { get; set; }
        public int ServicioId { get; set; }
        public string Tipo { get; set; }
        public string TipoServicio { get; set; }
        public string Escolaridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public Inmueble inmueble { get; set; }
        public CatalogoServicios servicio { get; set; }
        public Usuarios usuario { get; set; }
    }
}
