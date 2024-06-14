using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasCelular
    {
        Task<List<IncidenciasCelular>> getIncidenciasCelular(int id);
        Task<int> IncidenciasCelular(IncidenciasCelular incidenciasCelular);
        Task<List<IncidenciasCelular>> ListIncidenciasTipoCelular(int id, string tipo);
        Task<int> IncidenciasTipoCelular(int id, string tipo);
        Task<int> ActualizaIncidencia(IncidenciasCelular incidenciasCelular);
        Task<int> EliminaTodaIncidencia(int id, string tipo);
        Task<int> EliminaIncidencia(int id);
    }
}
