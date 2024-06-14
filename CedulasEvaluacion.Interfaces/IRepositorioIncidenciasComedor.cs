using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasComedor
    {
        Task<List<IncidenciasComedor>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasComedor>> GetIncidencias(int id);
        Task<int> IncidenciasComedor(IncidenciasComedor incidencia);
        Task<int> ActualizaIncidencia(IncidenciasComedor incidencia);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
