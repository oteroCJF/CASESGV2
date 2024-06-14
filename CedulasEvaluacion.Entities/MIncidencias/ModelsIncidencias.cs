using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public class ModelsIncidencias
    {
        public List<IncidenciasAgua> agua { get; set;}
        public List<IncidenciasAnalisis> analisis { get; set; }
        public List<IncidenciasCelular> celular{ get; set; }
        public List<IncidenciasCelular> altaEntrega { get; set; }
        public List<IncidenciasCelular> altasentrega { get; set; }
        public List<IncidenciasCelular> bajaServicio { get; set; }
        public List<IncidenciasCelular> reactivacion { get; set; }
        public List<IncidenciasCelular> suspension { get; set; }
        public List<IncidenciasCelular> cambioPerfil { get; set; }
        public List<IncidenciasCelular> switcheoCard { get; set; }
        public List<IncidenciasCelular> cambioRegion { get; set; }
        public List<IncidenciasCelular> servicioVozDatos { get; set; }
        public List<IncidenciasCelular> diagnostico { get; set; }
        public List<IncidenciasCelular> reparacion { get; set; }
        public List<IncidenciasConvencional> convencional { get; set; }
        public List<IncidenciasConvencional> contratacion { get; set; }
        public List<IncidenciasConvencional> cableado { get; set; }
        public List<IncidenciasConvencional> entregaAparato { get; set; }
        public List<IncidenciasConvencional> cambioDomicilio { get; set; }
        public List<IncidenciasConvencional> reubicacion { get; set; }
        public List<IncidenciasConvencional> identificador { get; set; }
        public List<IncidenciasConvencional> instalaciónTroncal { get; set; }
        public List<IncidenciasConvencional> contratacionInternet { get; set; }
        public List<IncidenciasConvencional> habilitacionServicios { get; set; }
        public List<IncidenciasConvencional> cancelacionServicios { get; set; }
        public List<IncidenciasConvencional> reporteFallas { get; set; }
        public List<IncidenciasFumigacion> fumigacion { get; set; }
        public List<IncidenciasLimpieza> iLimpieza { get; set; }
        public List<IncidenciasMensajeria> mensajeria { get; set; }
        public List<IncidenciasMensajeria> recoleccion { get; set; }
        public List<IncidenciasMensajeria> entrega { get; set; }
        public List<IncidenciasMensajeria> acuses { get; set; }
        public List<IncidenciasMensajeria> malEstado { get; set; }
        public List<IncidenciasMensajeria> extraviadas { get; set; }
        public List<IncidenciasMensajeria> robadas { get; set; }
        public List<IncidenciasMuebles> muebles { get; set; }
        public List<IncidenciasResiduos> residuos { get; set; }
        public List<IncidenciasResiduos> incidenciasResiduos { get; set; }
        public List<IncidenciasResiduos> incidenciasManifiesto { get; set; }
        public List<IncidenciasTransporte> transporte { get; set; }
        public List<IncidenciasTraslado> traslado { get; set; }
        public List<IncidenciasComedor> comedor { get; set; }
    }
}
