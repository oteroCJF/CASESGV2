using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioDocuments
    {
        Task<ActaEntregaRecepcion> getDatosActa(int cedula, int servicio);
    }
}
