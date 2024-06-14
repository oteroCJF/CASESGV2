using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasFumigacion
    {
        Task<List<IncidenciasFumigacion>> GetIncidenciasPregunta(int id, int pregunta);
        Task<List<IncidenciasFumigacion>> GetIncidencias(int id);
        Task<int> IncidenciasFumigacion(IncidenciasFumigacion incidenciasFumigacion);
        Task<int> ActualizaIncidencia(IncidenciasFumigacion incidenciasFumigacion);
        Task<int> EliminaIncidencia(int id);
        Task<int> EliminaTodaIncidencia(int id, int pregunta);
    }
}
