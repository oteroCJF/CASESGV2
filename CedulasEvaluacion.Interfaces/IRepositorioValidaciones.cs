using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioValidaciones
    {
        Task<bool> ObtienePeriodoComedor(CedulaEvaluacion cedula);
        Task<bool> GetAccionesEntregables(string entregable, string estatus, int servicio);
        Task<bool> GetValidacionIncidenciaComedor(IncidenciasComedor comedor);
    }
}
