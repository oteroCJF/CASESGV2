using CedulasEvaluacion.Entities.MAvisos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAvisosCASESG
    {
        Task<List<AvisosCASESG>> GetAvisosCASESG();
        Task<AvisosCASESG> GetAvisosCASESGById(int aviso);
        Task<List<VAvisos>> GetAvisosCASESGByUsuario(int usuario);
        Task<List<AvisosPerfil>> GetPerfilesByAviso(int aviso);
        Task<UsuariosAvisos> GetAvisosVisibles(int usuario);//metodo para ver si el usuario desea ver los avisos al iniciar
        Task<int> insertaAvisoCASESG(AvisosCASESG aviso);
        Task<int> actualizaAvisoCASESG(AvisosCASESG aviso);
        Task<int> insertaAvisosPerfil(List<AvisosPerfil> avisosP, int aviso);
        Task<int> insertaAvisosUsuario(UsuariosAvisos usuarioA);
        Task<int> insertaUsuarioVisible(UsuariosAvisos avisosU);
        Task<int> eliminaUsuarioVisible(UsuariosAvisos avisosU);
        Task<int> eliminaAviso(AvisosCASESG aviso);
        Task<int> eliminaPerfilesAviso(int aviso);
        Task<int> eliminaPerfilAviso(int aviso,int perfil);
    }
}
