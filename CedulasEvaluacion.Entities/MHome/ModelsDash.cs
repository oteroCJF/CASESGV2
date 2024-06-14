using CedulasEvaluacion.Entities.MAvisos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MHome
{
    public partial class ModelsDash
    {
        public List<Dashboard> dashboards { get; set; }
        public List<VAvisos> avisos { get; set; }
        public UsuariosAvisos avisosU { get; set; }
    }
}
