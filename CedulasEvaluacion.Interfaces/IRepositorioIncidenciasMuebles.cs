using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasMuebles 
    {
        Task<List<IncidenciasMuebles>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasMuebles>> GetIncidencias(int id);
        Task<int> IncidenciasMuebles(IncidenciasMuebles incidenciasMuebles);
        Task<int> ActualizaIncidencia(IncidenciasMuebles incidenciasMuebles);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
