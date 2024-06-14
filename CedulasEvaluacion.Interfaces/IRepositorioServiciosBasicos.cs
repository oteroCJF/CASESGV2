using CedulasEvaluacion.Entities.MServiciosB;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioServiciosBasicos
    {
        Task<List<VServiciosBasicos>> GetServicioBasico(int Anio, int servicio);
        Task<int> insertaServicioBasico(ServicioBasico sb);
        Task<int> actualizaServicioBasico(ServicioBasico sb);
        //Task<int> eliminaServicioBasico(ServicioBasico sb);
        Task<int> enviaServicioBasico(ServicioBasico sb);
        Task<ServicioBasico> GetServicioBasicoById(int id);

    }
}
