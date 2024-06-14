using CedulasEvaluacion.Entities.MDeductivas;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioReportesFinancieros
    {
        Task<List<ReporteCedula>> GetCedulasFinancieros(string mes, int anio);
        Task<List<ReporteCedula>> GetReportePagos(string mes, int anio);
        Task<List<ReporteCedula>> GetReporteServiciosFacturas(int servicio, string mes);
        Task<List<DesgloceDeductivas>> GeneraDesgloceCedula(int cedula, int servicio);
        Task<List<VFacturas>> GetReporteFacturas(int cedula, int servicio);
        Task<List<VConceptos>> GetReporteConceptosFactura(int factura);
    }
}
