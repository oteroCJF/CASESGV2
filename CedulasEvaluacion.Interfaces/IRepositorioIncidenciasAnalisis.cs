using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface  IRepositorioIncidenciasAnalisis
    {
        Task<List<IncidenciasAnalisis>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasAnalisis>> GetIncidencias(int id);
        Task<int> IncidenciasAnalisis(IncidenciasAnalisis incidenciasAgua);
        Task<int> ActualizaIncidencia(IncidenciasAnalisis incidenciasAgua);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
