using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class Preguntas
    {
        public int Id { get; set; }
        public int NoPreguntaUsuario { get; set; }
        public int NoPregunta { get; set; }
        public int ServicioId { get; set; }
        public string Pregunta { get; set; }
        public string Ayuda { get; set; }
        public string Respuesta { get; set; }
        public string Frecuencia { get; set; }
        public string Abreviacion { get; set; }
        public string Concepto { get; set; }
        public bool Cierre { get; set; }
        public bool NoAplica { get; set; }
        public bool NoRealizo { get; set; }
        public bool NoEntrego { get; set; }
        public bool Incidencias { get; set; }
        public bool Fecha { get; set; }
        public bool Fechas { get; set; }
        public bool SUA { get; set; }
        public bool Numero { get; set; }
    }
}
