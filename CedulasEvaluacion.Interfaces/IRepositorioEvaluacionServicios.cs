using System.Collections.Generic;
using System;
using System.Text;
using CedulasEvaluacion.Entities.Vistas;
using System.Threading.Tasks;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEvaluacionServicios
    {
        Task<int> VerificaCedula(CedulaEvaluacion cedula);
        Task<int> insertaCedula(CedulaEvaluacion cedula);
        Task<string> GetFolioCedula(int servicio);
        Task<CedulaEvaluacion> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int servicio, int cedula);
        Task<int> apruebaRechazaCedula(CedulaEvaluacion cedula);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorial(object id);
        Task<int> EliminaCedula(int id);
        int CuentaCedulasUrgentes(List<VCedulasEvaluacion> cedulas);
        Task<bool> GetPreguntaCapacitacion(int cedula);
        Task<bool> GetEstatusCedula(int cedula);
        Task<List<Penalizaciones>> GetPenalizacionesByCedula(int cedula,int servicio);
        Task<decimal> SumaDeductivas(int cedula, int servicio);
        Task<decimal> SumaPenalizaciones(int cedula, int servicio);
        Task<List<VCedulas>> GetCedulasEvaluacion(ModelsIndex cedula,int usuario);
        Task<int> SolicitudNC(CedulaEvaluacion cedula);
        Task<List<HistorialNotasCredito>> HistorialNotasCredito(int cedula);
        Task<int> autorizarRechazarSNC(HistorialNotasCredito cedula);
        Task<int> SolicitudRechazoCedula(CedulaEvaluacion cedula);
        Task<int> AutorizarRechazoCedula(CedulaEvaluacion cedula);
        Task<string> getPlantillaCorreo(int cedula);
        Task<string> getPlantillaCorreoSNC(int cedula);
        Task<string> getPlantillaCorreoSNCSD(int cedula);
    }
}
