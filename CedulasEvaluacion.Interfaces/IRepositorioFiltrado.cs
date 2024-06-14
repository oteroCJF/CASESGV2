using CedulasEvaluacion.Entities.MCedula;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFiltrado
    {
        Task<List<CedulaEvaluacion>> GetEstatusEvaluacion(int servicio, int usuario);
        Task<List<CedulaEvaluacion>> GetMesesEvaluacion(int servicio, int usuario);
    }
}
