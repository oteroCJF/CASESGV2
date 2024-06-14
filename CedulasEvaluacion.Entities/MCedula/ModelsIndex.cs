using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public class ModelsIndex
    {
        public List<VCedulasEvaluacion> cedulasEstatus { get; set; }
        public List<VCedulasEvaluacion> cedulasMes { get; set; }
        public List<VCedulas> cedulas { get; set; }
        public List<Variables> anios { get; set; }
        public List<Variables> TipoServicio { get; set; }
        public List<CedulaEvaluacion> festatus { get; set; }
        public List<CedulaEvaluacion> Meses { get; set; }
        public List<Inmueble> administraciones { get; set; }
        public List<Inmueble> inmuebles { get; set; }
        public int ServicioId { get; set; }
        public string Estatus { get; set; }
        public int Anio { get; set; }
        public int AdministracionId { get; set; }
        public int InmuebleId { get; set; }
        public int InmuebleDestinoId { get; set; }
        public string Mes { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public string Url { get; set; }

        /********************** Nuevas Variables Para Filtros *****************************/
        public List<string> filtroAnios { get; set; }
        public List<string> filtroMeses { get; set; }
        public List<string> filtroEstatus{ get; set; }
        public List<string> filtroAdmins { get; set; }
        public List<string> filtroInmuebles { get; set; }
        public List<string> filtroRegiones{ get; set; }
        public List<string> filtroTipoInm { get; set; }
        public List<string> filtroTipoServicio { get; set; }
        public List<string> filtroCalificacion { get; set; }
        public string filtroPeriodo { get; set; }
    }
}
