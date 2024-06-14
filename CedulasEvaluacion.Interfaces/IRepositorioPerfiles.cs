using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioPerfiles
    {
        Task<List<Perfiles>> getPerfiles();
        Task<int> insertarPerfil(Perfiles perfiles);
        Task<Perfiles> getPerfilById(int id);
        Task<int> actualizaPerfil(Perfiles perfiles);
        Task<int> getPermiso(int user, string modulo, string operacion);
        Task<List<Perfiles>> getPerfilesByUser(int user);
        Task<int> eliminaPerfilByUser(int id, int user);
    }
}
