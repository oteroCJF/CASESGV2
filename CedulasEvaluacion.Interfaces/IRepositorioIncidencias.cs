using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidencias
    {
        Task<List<IncidenciasLimpieza>> getIncidencias(int cedulaId);
        Task<List<IncidenciasLimpieza>> getIncidenciasByPregunta(int cedulaId, int pregunta);
        Task<int> insertarIncidencia(IncidenciasLimpieza incidencia);
        Task<int> actualizarIncidencia(IncidenciasLimpieza incidencia);
        Task<int> eliminarIncidencia(int id);
        Task<int> eliminarTodaIncidencia(int cedula, int pregunta);
    }
}
