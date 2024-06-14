using CedulasEvaluacion.Entities.MCargasMasivas;
using CedulasEvaluacion.Entities.MFacturas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCargasMasivas
    {
        Task<List<CargasMasivas>> getCargasMasivas();
        Task<CargasMasivas> getCargaMasivaById(int carga);
        Task<int> insertaCargaMsiva(CargasMasivas carga);
        Task<List<Facturas>> getFacturas(int carga);
        Task<List<Concepto>> getConceptosFactura(int factura);
        Task<Facturas> insertaFacturas(Facturas facturas);
        Task<int> insertaConceptoFacturas(Facturas facturas);
        Task<int> procesarArchivos(int carga, string tipo);
    }
}
