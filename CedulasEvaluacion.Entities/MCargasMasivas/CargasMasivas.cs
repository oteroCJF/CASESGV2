using CedulasEvaluacion.Entities.MFacturas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCargasMasivas
{
    public class CargasMasivas
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
        public int Anio { get; set; }
        public string Nombre { get; set; }
        public string TipoArchivo { get; set; }
        public string Estatus { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }

        /*********/
        public int TotalFacturas { get; set; }
        public List<Facturas> facturas { get; set; }
    }
}
