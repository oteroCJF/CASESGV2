using CedulasEvaluacion.Entities.Login;
using CedulasEvaluacion.Entities.MPerfiles;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Services
{
    public class ServiceModulos
    {
        private readonly IRepositorioLogin vRepositorioLogin;

        public ServiceModulos(IRepositorioLogin iRepositorioLogin)
        {
            this.vRepositorioLogin = iRepositorioLogin ?? throw new ArgumentNullException(nameof(iRepositorioLogin));
        }

        public async Task<List<VModulosUsuario>> GetVModulos(int user)
        {
            List<VModulosUsuario> modulos = await vRepositorioLogin.getModulosByUser(user);
            return modulos;
        }

        public async Task<List<ResponsablesDAS>> GetResponsablesDAS()
        {
            List<ResponsablesDAS> responsables= await vRepositorioLogin.GetResponsablesDAS();
            return responsables;
        }

    }
}
