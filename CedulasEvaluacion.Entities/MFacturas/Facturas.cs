using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class Facturas
    {
        public int Id { get; set; }
        public int CargaMasivaId { get; set; }
        public int CedulaId { get; set; }
        public int SBasicosId { get; set; }
        public int ServicioId { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string NombreArchivo { get; set; }
        public string FolioFactura { get; set; }
        public IFormFile Xml { get; set; }
        public int CedulaExistente { get; set; }
        public bool Retenciones { get; set; }
        public bool TipoServicio { get; set; }
        public Comprobante comprobante { get; set; }
        public Emisor emisor { get; set; }
        public List<Concepto> concepto { get; set; }
        public Concepto mconcepto { get; set; }
        public TimbreFiscal timbreFiscal { get; set; }
        public DatosTotales datosTotales { get; set; }
        public DatosExtra datosExtra { get; set; }
        public Traslado traslado { get; set; }
        public Receptor receptor{ get; set; }
        public Retencion retencion { get; set; }
        public CFDIRelacionado cfdiRelacionado { get; set; }
        public Impuestos impuestos { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }

        /*Parámetros adicionales para las Cargas Masivas*/
        public string Servicio { get; set; }
        public string Inmueble { get; set; }
        public string FolioCedula { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public bool Existe { get; set; }
        public decimal TotalDeductivas { get; set; }

    }
}
