using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregables
    {
        /*Metodos para adjuntar archivos*/
        Task<List<Entregables>> getEntregables(int cedula);

        Task<int> entregableFactura(Entregables entregables);

        Task<int> eliminaArchivo(Entregables entregable);

        Task<int> buscaEntregable(int id, string tipo);

        /*FIN de los metodos para adjuntar archivos*/

        /*Respuestas Adicionales*/
        Task<int> insertaCapacitacion(RespuestasAdicionales respuestasAdicionales);
        Task<int> insertaEntregablesBD(RespuestasAdicionales respuestasAdicionales);
        Task<RespuestasAdicionales> getCapacitacionLimpieza(int id);
        Task<List<RespuestasAdicionales>> getEntregablesBDLimpieza(int id);
        Task<int> eliminaEntregableBD(RespuestasAdicionales respuestasAdicionales);
        Task<int> calculaCalificacionEntregableBD(int cedula);
        Task<int> capturaHistorial(HistorialEntregables historialEntregables);
        Task<List<HistorialEntregables>> getHistorialEntregables(object id);
        Task<int> apruebaRechazaEntregable(Entregables entregables);
    }
}
