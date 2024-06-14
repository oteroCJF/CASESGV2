using CedulasEvaluacion.Entities.MCedulaR;
using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCedulaResultados
    {
        Task<IEnumerable<PeriodoEvaluacion>> GetPeriodoEvaluacion(int anio, int mesI, int mesF, int servicio, int contrato);
        Task<IEnumerable<MIndiceEfectividad>> GetIndiceEfectividad(int servicio, int anio, int mesI, int mesF, string rubros, int contrato);
        Task<IEnumerable<Variables>> GetRubrosaEvaluar(string rubros);
        Task<IEnumerable<MIndiceEfectividad>> GetIncidenciasInmueble(int servicio, int anio, int mesI, int mesF, string rubros, int contrato);
        Task<IEnumerable<AvanceFinanciero>> GetAvanceFinanciero(int servicio, int contrato);
        Task<IEnumerable<AvanceFisico>> GetAvanceFisico(int servicio, int contrato);
    }
}
