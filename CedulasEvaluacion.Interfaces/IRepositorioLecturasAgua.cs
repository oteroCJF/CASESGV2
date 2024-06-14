using CedulasEvaluacion.Entities.MLecturasAgua;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioLecturasAgua
    {
        Task<List<DashboardLectura>> GetLecturas(int anio, int user);
        Task<List<LecturaAgua>> GetLecturasByInmueble(int anio, int inmueble);
        Task<LecturaAgua> GetLecturaById(int id);
        Task<int> insertaLectura(LecturaAgua lectura);
        Task<int> actualizaLectura(LecturaAgua lectura);
        Task<int> eliminaLectura(int id);
    }
}
