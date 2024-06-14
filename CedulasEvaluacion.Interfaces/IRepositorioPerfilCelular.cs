using CedulasEvaluacion.Entities.MIncidencias;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioPerfilCelular
    {
        Task<List<PerfilesCelular>> GetPerfilesCelular();
        Task<PerfilesCelular> GetPerfilCelularById(int id);
    }
}
