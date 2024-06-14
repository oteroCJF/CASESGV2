using CedulasEvaluacion.Entities.MPerfiles;
using CedulasEvaluacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Services
{
    public class ServicePermisos
    {
        private readonly IRepositorioOperacionesPerfil vPermisos;
        private readonly IRepositorioValidaciones vValidaciones;

        public ServicePermisos(IRepositorioOperacionesPerfil viPermisos, IRepositorioValidaciones iValidaciones)
        {
            this.vPermisos = viPermisos ?? throw new ArgumentNullException(nameof(viPermisos));
            this.vValidaciones = iValidaciones ?? throw new ArgumentNullException(nameof(iValidaciones));
        }

        public async Task<int> GetPermisosByModulo(string permiso, int servicio, int usuario)
        {
            PermisosPerfil modulos = await vPermisos.GetPermisoModuloByUser(permiso, servicio, usuario);
            return modulos.Servicio;
        }
        public async Task<bool> GetAccionesEntregables(string entregable, string estatus, int servicio)
        {
            bool acciones = await vValidaciones.GetAccionesEntregables(entregable, estatus, servicio);
            return acciones;
        }
    }
}
