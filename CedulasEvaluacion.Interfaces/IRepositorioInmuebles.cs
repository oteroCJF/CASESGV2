using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioInmuebles
    {
        Task<List<Inmueble>> getAdministraciones();
        Task<List<Inmueble>> getAdministracionesByServicio(int servicio,int usuario);
        Task<List<Inmueble>> getInmuebles();
        Task<Inmueble> inmuebleById(int id);
        Task<int> insertAdmin(Inmueble inmueble);
        Task<int> updateAdmin(Inmueble inmueble);
        Task<int> deleteAdmin(int id);
        Task<List<Inmueble>> getInmueblesByAdmin(int id);
        Task<List<Inmueble>> getInmueblesAEvaluar(int user);
        Task<int> insertaDireccionBM(Inmueble inmueble);        
        Task<List<Inmueble>> getEstadosRM();
        Task<List<Inmueble>> getDireccionesAlternasBM();

        Task<List<Inmueble>> getFiltrosInmuebles(int user, int servicio);
        Task<List<Inmueble>> getComedoresAEvaluar(int user);
    }
}
