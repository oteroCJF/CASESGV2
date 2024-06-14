using CedulasEvaluacion.Entities.Login;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Task<List<Usuarios>> getUsuarios();
        Task<Usuarios> getUserById(int id);
        Task<int> insertaUsuario(string datosUsuario, string password);
        Task<List<InmueblesUsuarios>> getInmueblesUsuario(int user);
        Task<int> insertaAdminByUser(List<InmueblesUsuarios> inmueblesUsuarios);
        Task<int> EliminaAdminByUser(int usuario, int administracion);
        Task<int> actualizaCorreoElectronico(Usuarios usuarios);
        Task<int> asignaPerfil(List<PerfilesUsuario> perfilesUsuario);
        Task<List<DestinatariosCorreo>> getDestinatariosCorreo(int servicio);
    }
}
