using CedulasEvaluacion.Entities.MFirmantes;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Reportes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFirmantes
    {
        Task<List<FirmantesServicio>> GetInmueblesFirmante(int servicio, int usuario);
        Task<List<ReporteCedula>> GetFirmantesByCedula(int cedula, int servicio);
        Task<List<Usuarios>> GetUsuariosByAdministracion(int user);
        Task<int> GetVerificaFirmantes(string tipo, int inmueble, int servicio, int cedula);
        Task<int> insertaFirmante(FirmantesServicio firmante);
        Task<int> actualizaFirmantes(FirmantesServicio firmante);
    }
}
