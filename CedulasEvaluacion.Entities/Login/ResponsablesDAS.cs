using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Login
{
    public class ResponsablesDAS
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Servicios{ get; set; }
        public string Horario { get; set; }
        public string Puesto { get; set; }
        public int Extension{ get; set; }
    }
}
