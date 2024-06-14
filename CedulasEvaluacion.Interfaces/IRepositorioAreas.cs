using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAreas
    {
        Task<Areas> getAreasById(int area);
        Task<int> insertaArea(Areas area);
    }
}
