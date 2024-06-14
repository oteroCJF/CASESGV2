using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Entities.MServiciosB
{
	public class ServicioBasico
	{
		public int Id { get; set; }
		public int UsuarioId { get; set; }
		public int ServicioId { get; set; }
		public int InmuebleId { get; set; }
		public int Anio { get; set; }
		public decimal TotalMontoFactura { get; set; }
		public string Mes { get; set; }
		public string Estatus { get; set; }
		public string NombreServicio { get; set; }
		public string NombrePDF { get; set; }
		public IFormFile FacturaPDF { get; set; }
		public DateTime FechaCreacion { get; set; }
		public DateTime FechaActualizacion { get; set; }
		public DateTime FechaEliminacion { get; set; }
		public CatalogoServicios servicio { get; set; }
		public List<Facturas> facturas { get; set; }
		public Inmueble inmuebles { get; set; }
		public List<Entregables> entregables { get; set; }

	}
}
