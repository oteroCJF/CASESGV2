using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasMensajeria
    {
        Task<int> IncidenciasExcel(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> IncidenciasMensajeria(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> IncidenciaRobada(IncidenciasMensajeria incidenciasMensajeria);
        Task<List<IncidenciasMensajeria>> getIncidenciasMensajeria(int id);
        Task<List<IncidenciasMensajeria>> getIncidenciasByTipoMensajeria(int id,string tipo);
        Task<int> ActualizaIncidencia(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> ActualizaRobada(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> EliminaIncidencia(int id);
        Task<List<IncidenciasMensajeria>> TotalIncidencias(int id);
        Task<List<IncidenciasMensajeria>> TotalIncidenciasByPregunta(int id, int pregunta);
        Task<int> EliminaTodaIncidencia(int id,string tipo);
        Task<int> IncidenciasTipo(int id, string tipo);
        Task<List<VSeguimiento>> getSeguimientosEstafeta();
        Task<VSeguimiento> getSeguimientosEstafetaById(int id);
        Task<List<IncidenciasMensajeria>> getIncidenciasMensajeriaSeguimiento(int id);
        Task<int> InsertaReferenciasSeguimiento(ReferenciaPago referencia);
        Task<List<ReferenciaPago>> getReferenciasPago(int incidencia);
        Task<int> actualizaReferenciasSeguimiento(ReferenciaPago referencia);
        Task<int> insertaReciboPago(ReferenciaPago referencia);
    }
}
