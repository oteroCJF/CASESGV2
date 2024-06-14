using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasAgua
    {
        Task<List<IncidenciasAgua>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasAgua>> GetIncidencias(int id);
        Task<int> IncidenciasAgua(IncidenciasAgua incidenciasAgua);
        Task<int> ActualizaIncidencia(IncidenciasAgua incidenciasAgua);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
