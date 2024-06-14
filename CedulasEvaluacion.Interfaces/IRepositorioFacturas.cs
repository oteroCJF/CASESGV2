using CedulasEvaluacion.Entities.MFacturas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFacturas
    {
        Task<Facturas> insertaFacturas(Facturas facturas);
        Task<int> insertaConceptoFacturas(Facturas facturas);
        Task<Facturas> updateFacturas(Facturas facturas);
        Task<int> updateConceptoFacturas(Facturas facturas);
        Task<List<Facturas>> getFacturas(int cedula,int servicio);
        Task<List<Facturas>> getFacturasSB(int sb, int servicio);
        Task<List<Concepto>> getConceptosFactura(int factura);
        Task<int> deleteFactura(int factura);
        decimal obtieneTotalFacturas(List<Facturas> facturas);
        Task<List<Facturas>> getFacturasPago(int servicio);
        Task<Facturas> getFacturaById(int id);

        /*********** Facturas de Servicios Basicos *************/
        Task<Facturas> insertaFacturasSB(Facturas facturas);
        Task<int> insertaConceptoFacturasSB(Facturas facturas);
        Task<Facturas> updateFacturasSB(Facturas facturas);
        Task<int> updateConceptoFacturasSB(Facturas facturas);

        /******************* Módulo de Facturas *******************/
        Task<List<DashboardFacturas>> getFacturasTipo(string tipo,int anio);
        Task<List<DesgloceServicio>> getDesgloceFacturacion(int servicio,int anio);
        Task<List<DesgloceServicio>> getResumenFacturacion(int servicio,int anio);

        Task<int> insertaMasivoConceptoFacturas(Facturas facturas, string folio, string tipo, int servicio);


    }
}
