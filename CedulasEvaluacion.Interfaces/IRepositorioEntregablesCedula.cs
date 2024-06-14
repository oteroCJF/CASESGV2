using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesCedula
    {
        /*Metodos para adjuntar archivos*/
        Task<List<Entregables>> getEntregables(int cedula,int servicio);
        Task<int> adjuntaEntregable(Entregables entregables);//adjuntaEntregable
        Task<int> eliminaEntregable(Entregables entregable);
        Task<int> eliminaEntregables(Entregables entregable);
        Task<int> GetFlujoCedulaCAE(int cedula, string estatus);
        Task<int> GetFlujoCedulaCAR(int cedula, string estatus);
        Task<int> buscaEntregable(int id, string tipo);
        Task<int> capturaHistorial(HistorialEntregables historialEntregables);
        Task<List<HistorialEntregables>> getHistorialEntregables(int id,int servicioId);
        Task<int> apruebaRechazaEntregable(Entregables entregables);
        Task<List<Entregables>> GetAlcancesEntregable(int id);
        Task<int> validaCedulaDAS(int id);
        Task<List<Entregables>> obtieneActasEntregaRecepcion(int anio, string mes);
        Task<List<Entregables>> DescargarEntregables(List<Entregables> entregables);
    }
}
