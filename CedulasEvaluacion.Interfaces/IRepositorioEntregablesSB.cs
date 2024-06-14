using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesSB
    {
        Task<List<Entregables>> GetEntregablesSB(int id);
        Task<int> insertaEntregableSB(Entregables entregables);
        Task<int> actualizaEntregableSB(Entregables entregables);
        Task<int> eliminaEntregable(Entregables sb);
    }
}
