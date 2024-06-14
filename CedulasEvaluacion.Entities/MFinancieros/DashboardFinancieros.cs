
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class DashboardFinancieros
    {
        public string Estatus { get; set; }
        public string Mes { get; set; }
        public string Servicio { get; set; }
        public string Descripcion { get; set; }
        public string Fondo { get; set; }
        public string Icono { get; set; }
        public int Total { get; set; }
        public int TotalParcial { get; set; }
        public int APendientes { get; set; }
        public int CedulasPendientes { get; set; }
        public int MemosPendientes { get; set; }
        public int ServicioId { get; set; }
        public List<Oficio> oficios { get; set; }
    }
}
