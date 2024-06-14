using CedulasEvaluacion.Entities.MContratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioContratosServicio
    {
        Task<List<ContratosServicio>> GetContratosServicios(int servicio);
        Task<ContratosServicio> GetContratoServicioActivo(int servicio);
        Task<ContratosServicio> GetContratoServicioById(int id);
        Task<int> InsertaContrato(ContratosServicio contratosServicio);
        Task<int> ActualizaContrato(ContratosServicio contratosServicio);
        Task<int> InsertarActualizarDocumentacion(EntregablesContrato entregables);
        Task<int> eliminarContrato(int contrato);
        Task<int> eliminarConvenio(int convenio);
        Task<int> eliminaArchivo(EntregablesContrato entregables);
        Task<List<ConveniosContrato>> GetConveniosByContrato(int contrato);
        Task<int> InsertaConvenio(ConveniosContrato convenio);
        Task<int> ActualizaConvenio(ConveniosContrato convenio);
    }
}
