using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Reportes
{
    public partial class ReporteCedula
    {
        public int Id { get; set; }
        public string Inmueble { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public string Incumlimiento { get; set; }
        public string Empresa { get; set; }
        public int Anio { get; set; }
        public string Administracion { get; set; }
        public string Servicio { get; set; }
        public string TipoServicio { get; set; }
        public string Estatus { get; set; }
        public string EstatusFactura { get; set; }
        public string Calificacion { get; set; }
        public string Reviso { get; set; }
        public string PuestoReviso { get; set; }
        public string Superviso { get; set; }
        public string PuestoSuperviso { get; set; }
        public string Elaboro { get; set; }
        public string Autoriza { get; set; }
        public string FechaCreacion { get; set; }
        public string Periodo { get; set; }
        public string Facturas { get; set; }
        public string Sexo { get; set; }
        public string MontosFacturas { get; set; }
        public string NumeroOficio { get; set; }
        public int TotalCedulas { get; set; }
        public int DiasTranscurridos { get; set; }
        public DateTime FechaTramitado { get; set; }
        public DateTime FechaPagado { get; set; }
    }
}
