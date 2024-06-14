using CedulasEvaluacion.Entities.MLecturasAgua;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesLecturasAgua
    {
        Task<List<EntregablesLecturasAgua>> getEntregablesByLectura(int lectura);
        Task<int> adjuntaEntregable(EntregablesLecturasAgua entregables);//adjuntaEntregable
        Task<int> eliminaEntregable(EntregablesLecturasAgua entregable);
        Task<int> eliminaEntregables(EntregablesLecturasAgua entregable);
    }
}
