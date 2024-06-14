using CedulasEvaluacion.Entities.MCatalogoServicios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCatalogoServicios
    {
        //Catalogo de Servicios
        Task<List<DashboardCS>> GetDashBoard();
        Task<List<CatalogoServicios>> GetCatalogoServicios();
        Task<CatalogoServicios> GetServicioById(int servicio);
        Task<CatalogoServicios> GetDescripcionServicio(string servicio);
        Task<List<CatalogoServicios>> GetServiciosByUsers(int usuario);
    }
}
