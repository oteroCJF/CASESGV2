using CedulasEvaluacion.Entities.MCedula;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCuestionarios
    {
        Task<List<Preguntas>> GetCuestionarioCompleto(int servicio);
        Task<List<Preguntas>> GetCuestionarioByServicio(int servicio, int cedula);
    }
}
