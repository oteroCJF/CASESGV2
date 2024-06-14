using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasTraslado
    {
        Task<int> InsertaIncidencia(IncidenciasTraslado incidenciasTraslado);
        Task<int> ActualizaIncidencia(IncidenciasTraslado incidenciasTraslado);
        Task<List<IncidenciasTraslado>> getIncidencias(int id);
        Task<List<IncidenciasTraslado>> getIncidenciasByPregunta(int cedulaId, int pregunta);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
