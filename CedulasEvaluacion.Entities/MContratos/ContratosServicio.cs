using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MContratos
{
    public partial class ContratosServicio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroContrato { get; set; }
        public string Empresa { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Representante { get; set; }
        public string ContratoPDF { get; set; }
        public string Anexos { get; set; }
        public string JuntaAclaraciones { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public decimal MontoMin { get; set; }
        public decimal MontoMax { get; set; }
        public int VolumetriaMin { get; set; }
        public int VolumetriaMax { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }

        public List<ConveniosContrato> convenios { get; set; } = new List<ConveniosContrato>();

    }
}
