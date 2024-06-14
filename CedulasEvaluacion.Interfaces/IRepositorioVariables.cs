using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioVariables
    {
        Task<List<Variables>> GetAniosEvaluacion();
        Task<List<Variables>> GetTipoServicio(int servicio);
        Task<List<Variables>> GetListadoAlcances(int servicio, int cedula);
        Task<List<Variables>> GetListadoEntregables(int servicio);
        Task<List<Variables>> GetEntregablesByServicioCAE(int servicio, int cedula);
        Task<List<Variables>> GetEntregablesByServicioCAR(int servicio, int cedula);
        Task<List<Variables>> GetEntregablesUpdateByServicio(int servicio,string tipo);
        Task<Variables> GetHorarioComedor(int comedor, int servicio);
        Task<List<Variables>> GetTiposIncidenciasByServicio(int servicio);
        Task<List<Variables>> GetDetalleIncidenciaByTipo(int servicio, string tipo);
        Task<List<Variables>> GetPestanasRevisionCedula(int servicio, int cedula);
        Task<List<Meses>> GetMesesEvaluacion();
        Task<List<Variables>> GetRubrosEvaluar(int servicio);
    }
}
