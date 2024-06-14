using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAlcancesCedula
    {
        Task<List<CedulaEvaluacion>> getCedulasByMes(int servicio, int anio);
        Task<int> habilitaAlcancesCedulas(CedulaEvaluacion cedula);
        Task<int> capturaHistorial(HistorialEntregables historialEntregables);
        Task<List<HistorialEntregables>> getHistorialEntregables(int id, int servicioId);
        Task<int> apruebaRechazaAlcance(Entregables entregables);
    }
}
