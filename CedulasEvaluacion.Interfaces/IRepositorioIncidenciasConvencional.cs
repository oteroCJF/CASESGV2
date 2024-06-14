using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasConvencional
    {
        Task<List<IncidenciasConvencional>> getIncidenciasConvencional(int id);
        Task<int> InsertaIncidencia(IncidenciasConvencional incidenciasConvencional);
        Task<int> ActualizaIncidencia(IncidenciasConvencional incidenciasConvencional);
        Task<int> EliminaIncidencia(int id);
        Task<int> IncidenciasTipoConvencional(int id, string tipo);
        Task<int> EliminaTodaIncidencia(int id, string tipo);
        Task<List<IncidenciasConvencional>> ListIncidenciasTipoConvencional(int id, string v);
    }
}
