using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class Comprobante
    {

        public string Serie { get; set; }
        public long Folio { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
