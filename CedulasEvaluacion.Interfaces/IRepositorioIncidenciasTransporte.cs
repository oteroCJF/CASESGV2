using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasTransporte
    {
        Task<List<IncidenciasTransporte>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasTransporte>> GetIncidencias(int id);
        Task<int> IncidenciasTransporte(IncidenciasTransporte incidenciasTransporte);
        Task<int> ActualizaIncidencia(IncidenciasTransporte incidenciasTransporte);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
